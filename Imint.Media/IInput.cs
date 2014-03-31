// 
//  IInput.cs
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
using Raster = Kean.Draw.Raster;
using Parallel = Kean.Parallel;
using Geometry2D = Kean.Math.Geometry2D;
using Collection = Kean.Collection;

namespace Imint.Media
{
	public interface IInput :
		IDisposable
	{
		Action<Frame> Send { set; }
		event Action Resetting;
		void Initialize(Parallel.ThreadPool threadPool);
	}
}
