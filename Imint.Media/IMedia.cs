// 
//  IMedia.cs
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
using Uri = Kean.Core.Uri;
using Geometry2D = Kean.Math.Geometry2D;

namespace Imint.Media
{
	public interface IMedia :
		IControl
	{
		Kean.Math.Fraction Ratio { get; set; }
		event Action<Kean.Math.Fraction> RatioChanged;
		Media.Scan Scan { get; set; }
		event Action<Media.Scan> ScanChanged;
		Geometry2D.Integer.Shell Crop { get; set; }
		event Action<Geometry2D.Integer.Shell> CropChanged;
	}
}
