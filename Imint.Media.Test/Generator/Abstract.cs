// 
//  Abstract.cs
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

using Kean.Core.Extension;
using System;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Uri = Kean.Core.Uri;

namespace Imint.Media.Test.Generator
{
	public abstract class Abstract :
		Collection.Abstract.ReadOnlyVector<Tuple<Raster.Image, Tuple<string, object>[]>>,
		IDisposable
	{
		public abstract string Name { get; }
		public abstract void Open(Uri.Locator argument);
		public abstract void Close();

		public void Dispose()
		{
			this.Close();
			foreach (Tuple<Raster.Image, Tuple<string, object>[]> item in this)
				if (item.Item1.NotNull())
					item.Item1.Dispose();
		}
	}
}
