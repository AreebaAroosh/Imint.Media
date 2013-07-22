// 
//  Stream.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2010-2013 Imint AB
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Kean.Core;
using Kean.Core.Collection.Extension;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Serialize = Kean.Core.Serialize;
using Uri = Kean.Core.Uri;
using Kean.Core.Extension;

namespace Imint.Media.Photo
{
	public class Stream :
		Media.Player.IStream,
		Media.Player.IFile
	{
		public string[] SupportedExtensions { get { return new string[] { "png", "jpg", "jpeg" }; } }

		/// <summary>
		/// The number of frames per second.
		/// </summary>
		[Serialize.Parameter]
		public Kean.Math.Fraction Rate { get; set; }
		DateTime currentTime = new DateTime();
		/// <summary>
		/// The duration of one frame.
		/// </summary>
		protected TimeSpan duration;
		Raster.Image[] photos;
		/// <summary>
		/// The number of frames in the sequence.
		/// </summary>
		protected int Count { get { return (this.photos.NotNull()) ? photos.Length : 0; } }
		int index = 0;
		/// <summary>
		/// The index of the frame currently being shown.
		/// </summary>
		protected int Index { get { lock (this.signal) return this.index; } set { lock (this.signal) this.index = value; } }
		/// <summary>
		/// The timer that keeps track of when to update frames.
		/// </summary>
		protected System.Timers.Timer Timer { get; private set; }
		object signal = new object();
		/// <summary>
		/// Constructor.
		/// </summary>
		public Stream()
		{
			this.Rate = (Kean.Math.Fraction)25f;
		}
		#region IStream Members
		public int Channels { get { return 1; } }
		public Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { get; set; }
		public virtual Status Status { get { return (this.photos.NotNull() && this.Count == 0) ? Status.Closed : Status.Playing; } }
		public bool Open(Uri.Locator name)
		{
			bool result = false;
			if (name.Scheme == "file" && this.SupportedExtensions.Contains(name.Path.Extension))
			{
				this.photos = new Raster.Image[] { Raster.Image.Open(name.PlatformPath) };

				Kean.Math.Fraction rate = name.Query["rate"];
				if (rate.Nominator <= 0)
					rate = this.Rate;
				this.Timer = new System.Timers.Timer(1000 / (float)rate);
				this.duration = new TimeSpan((long)(10000 * 1000 / (float)rate));

				this.Timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs elapsedArguments) =>
				{
					lock (this.signal)
					{
						if (this.Count != 0)
							this.Index = (this.Index + 1) % this.Count;
						System.Threading.Monitor.Pulse(this.signal);
					}
				};
				result = true;
				this.Timer.Start();
			}
			return result;
		}
		public void Close()
		{
			if (this.Timer.NotNull())
			{
				this.Timer.Dispose();
				this.Timer = null;
			}
			if (this.photos.NotNull())
			{
				foreach (Raster.Image image in this.photos)
					if (image.NotNull())
						image.Dispose();
				this.photos = null;
			}
		}
		public void Poll()
		{
			lock (this.signal)
				if (System.Threading.Monitor.Wait(this.signal, 20))
					this.SendFrame();
		}
		/// <summary>
		/// Sends a frame on the stream's channels.
		/// </summary>
		protected virtual void SendFrame()
		{
			lock (this.signal)
			{
				this.Send(0, this.currentTime, this.duration, this.photos[this.Index].Copy() as Raster.Image, null);
				this.currentTime += this.duration;
			}
		}
		#endregion
		void IDisposable.Dispose()
		{
			this.Close();
		}

	}
}
