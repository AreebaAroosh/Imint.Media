// 
//  Module.cs
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
using Serialize = Kean.Core.Serialize;
using Uri = Kean.Core.Uri;
using Parallel = Kean.Core.Parallel;
using Platform = Kean.Platform;
using Geometry2D = Kean.Math.Geometry2D;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;

namespace Imint.Media.Input
{
	public class Module :
		Media.Module.InputControl
	{
		Unbuffered backend;
		[Serialize.Parameter("Player")]
		public Players Players 
		{ 
			get { return this.backend.Players; } 
			private set { this.backend.Players = value; } 
		}
		[Serialize.Parameter]
		public EndMode EndMode 
		{ 
			get { return this.backend.EndMode; }
			set { this.backend.EndMode = value; } 
		}
		public Module() :
			this(new Unbuffered())
		{ }
		Module(Unbuffered backend) :
			base(backend)
		{
			this.backend = backend;
		}
		protected override void Initialize()
		{
			this.Players.Application = this.Application;
			base.Initialize();
		}
	}
}
