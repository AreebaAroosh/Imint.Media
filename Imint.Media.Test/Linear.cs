// 
//  Linear.cs
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

namespace Imint.Media.Test
{
    public class Linear :
        Stream,
		Media.Player.ILinear
    {
		public override Status Status { get { return this.Count == 0 ? base.Status : this.Playing ? Status.Playing : Status.Paused; } }
        public bool Playing { get; private set; }
        public bool Play()
        {
            this.Timer.Start();
            this.Playing = true;
            return true;
        }
        public bool Pause()
        {
            this.Timer.Stop();
            this.Playing = false;
            return true;
        }
        public bool IsLinear { get { return true; } }
        public DateTime Position { get { return new DateTime(1000 / this.FramesPerSeconds * 10000 * this.Index); } }
		protected override void SendFrame()
		{
			lock (this.signal)
			{
				this.Send(0, this.Position, this.duration, this.generator[this.Index].Item1.Copy() as Kean.Draw.Raster.Image, this.generator[this.Index].Item2);
			}
		}
    }
}
