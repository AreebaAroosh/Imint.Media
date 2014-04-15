// 
//  NonLinear.cs
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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Uri = Kean.Uri;

namespace Imint.Media.Photo
{
	public class NonLinear :
		Linear,
		Media.Player.INonLinear
	{
		public bool IsNonLinear { get { return this.Count > 1000; } }
		public DateTime Start { get { return new DateTime(); } }
		public DateTime End { get { return new DateTime((long)(1000 / (float)this.Rate * 10000 * (this.Count - 1))); } }
		public void Seek(DateTime position)
		{
			this.Buffer.Seek(Kean.Math.Integer.Clamp((int)(position.Ticks / 10000 / (1000 / (float)this.Rate)), 0, this.Count - 1));
			if (!this.Playing)
				this.SendFrame();
		}
		public override bool Open(Uri.Locator name)
		{
			return base.Open(name) && (this.Buffer.Wrap || this.Pause());
		}
	}
}
