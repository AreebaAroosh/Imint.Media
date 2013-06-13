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

namespace Imint.Media.Test
{
	public class Stream :
		Media.Player.IStream,
		Media.Player.ICapture
	{
		public System.Collections.Generic.IEnumerable<Resource> Devices
		{
			get
			{
				foreach (Generator.Abstract generator in this.Generators)
					yield return new Media.Resource(ResourceType.Test, generator.Name, "test://" + generator.Name);
			}
		}
		Collection.IList<Generator.Abstract> generators = new Collection.List<Generator.Abstract>();
		[Serialize.Parameter("Generator")]
		public Collection.IList<Generator.Abstract> Generators { get { return this.generators; } }

		[Serialize.Parameter]
		public int FramesPerSeconds { get; set; }
		DateTime currentTime = new DateTime();
		protected TimeSpan duration;
		protected Generator.Abstract generator;
		protected int Count { get { return this.generator.NotNull() ? this.generator.Count : 0; } }
		protected System.Timers.Timer Timer { get; private set; }
		protected object signal = new object();
		int index = 0;
		protected int Index { get { lock (this.signal) return this.index; } set { lock (this.signal) this.index = value; } }
		public Stream()
		{
			this.FramesPerSeconds = 25;
			this.duration = new TimeSpan(0, 0, 0, 0, 1000 / this.FramesPerSeconds);
		}
		#region IStream Members
		public int Channels { get { return 1; } }
		public Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { get; set; }
		public virtual Status Status { get { return this.generator.IsNull() ? Status.Closed : Status.Playing; } }
		public bool Open(Uri.Locator name)
		{
			bool result = false;
			if (name.Scheme == "test" && this.generator.IsNull() && name.Authority.NotNull())
			{
				this.generator = this.Generators.Find(generator => generator.Name == name.Authority.Endpoint.Host);
				if (this.generator.NotNull())
				{
					this.generator.Open(name);
					Kean.Math.Fraction rate = name.Query["rate"];
					if (rate.Nominator > 0)
						this.Timer = new System.Timers.Timer(1000 / (float)rate);
					else
						this.Timer = new System.Timers.Timer(1000 / this.FramesPerSeconds);
					this.Timer.Elapsed += (object sender, System.Timers.ElapsedEventArgs elapsedArguments) =>
					{
						lock (this.signal)
						{
							if (this.generator.NotNull())
								this.Index = (this.Index + 1) % this.Count;
							System.Threading.Monitor.Pulse(this.signal);
						}
					};
					result = true;
					if (this is Stream && !(this is Linear))
						this.Timer.Start();
				}
			}
			return result;
		}
		public void Close()
		{
			if (this.generator.NotNull())
			{
				this.Timer.Stop();
				this.generator.Close();
				this.generator = null;
			}
		}
		public void Poll()
		{
			lock (this.signal)
				if (System.Threading.Monitor.Wait(this.signal, 20))
					this.SendFrame();
		}
		protected virtual void SendFrame()
		{
			lock (this.signal)
			{
				this.Send(0, this.currentTime, this.duration, this.generator[this.Index].Item1.Copy() as Kean.Draw.Raster.Image, this.generator[this.Index].Item2);
				this.currentTime += this.duration;
			}
		}
		#endregion
		void IDisposable.Dispose()
		{
			this.Close();
			if (this.Generators.NotNull())
			{
				foreach (Generator.Abstract generator in this.Generators)
					generator.Dispose();
			}
		}
	}
}
