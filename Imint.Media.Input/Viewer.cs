// 
//  Viewer.cs
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
using Kean.Core.Extension;
using Bitmap = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Error = Kean.Core.Error;
using Geometry2D = Kean.Math.Geometry2D;
using Log = Kean.Platform.Log;
using Parallel = Kean.Core.Parallel;
using Kean.Core.Collection.Extension;
using Uri = Kean.Core.Uri;
using Serialize = Kean.Core.Serialize;

namespace Imint.Media.Input
{
	public class Viewer :
		Buffered
	{
		System.Timers.Timer timer = new System.Timers.Timer(40);

		[Serialize.Parameter]
		public float FrameRate 
		{
			get { return (float) (1000.0 / this.timer.Interval); }
			set { this.timer.Interval = 1000.0f / value; } 
		}
		protected override bool Playing
		{
			get { return this.timer.Enabled; }
			set
			{
				if (value)
					this.timer.Start();
				else
					this.timer.Stop();
			}
		}
		public Viewer()
		{
			this.FrameRate = 25;
			this.timer.Elapsed += (sender, e) => { this.Send(); };
		}
	}
}
