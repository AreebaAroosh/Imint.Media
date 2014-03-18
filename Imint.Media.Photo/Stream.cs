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
using Kean;
using Kean.Collection.Extension;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Collection;
using Serialize = Kean.Serialize;
using Uri = Kean.Uri;
using Kean.Extension;
using System.Text.RegularExpressions;

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
		/// <summary>
		/// The duration of one frame.
		/// </summary>
		protected TimeSpan Duration { get; set; }
		protected Buffer.Abstract Buffer { get; set; }
		/// <summary>
		/// The number of frames in the sequence.
		/// </summary>
		protected int Count { get { return (this.Buffer.NotNull()) ? Buffer.Count : 0; } }
		int index = 0;
		/// <summary>
		/// The index of the frame currently being shown.
		/// </summary>
		protected int Index { get { lock (this.signal) return this.index; } set { lock (this.signal) this.index = value; } }
		/// <summary>
		/// The timer that keeps track of when to update frames.
		/// </summary>
		protected System.Timers.Timer Timer { get; private set; }
		public DateTime Position { get { return new DateTime((long)(1000 / (float)this.Rate * 10000 * this.Index)); } }
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
		// TODO: Something about this status - never returns Status.Paused.
		public virtual Status Status { get { return (this.Buffer.NotNull() && this.Count > 0) ? Status.Playing : Status.Closed; } }
		public bool Open(Uri.Locator name)
		{
			bool result = false;
			if (name.Scheme == "file" && this.SupportedExtensions.Contains(name.Path.Extension))
			{
				if ((this.Buffer = Photo.Buffer.Abstract.Open(name)).NotNull())
				{
					Kean.Math.Fraction rate = name.Query["rate"];
					if (rate.Nominator <= 0)
						rate = this.Rate;
					this.Timer = new System.Timers.Timer(1000 / (float)rate);
					this.Duration = new TimeSpan((long)(10000 * 1000 / (float)rate));

					this.Timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs elapsedArguments) =>
					{
						lock (this.signal)
						{
							System.Threading.Monitor.Pulse(this.signal);
						}
					};
					result = true;
					this.Timer.Start();
				}
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
			if (this.Buffer.NotNull())
			{
				this.Buffer.Close();
				this.Buffer = null;
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
		protected void SendFrame()
		{
			lock (this.signal)
			{
				Tuple<int, Raster.Image> next = this.Buffer.Next();
				if (next.Item2 == null)
				{
					this.Timer.Stop();
					this.Index = 0;
				}
				else
				{
					this.Index = next.Item1;
					this.Send(0, this.Position, this.Duration, next.Item2 as Raster.Image, null);
				}
			}
		}
		#endregion
		void IDisposable.Dispose()
		{
			this.Close();
		}

	}
}
