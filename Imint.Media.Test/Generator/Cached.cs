// 
//  Cached.cs
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
using Collection = Kean.Core.Collection;
using Raster = Kean.Draw.Raster;
using Serialize = Kean.Core.Serialize;
using Uri = Kean.Core.Uri;

namespace Imint.Media.Test.Generator
{
	public abstract class Cached :
		Abstract
	{
		public enum Colorspace
		{
			Bgra,
			Bgr,
			Yuv420,
			Yvu420,
			Yuyv,
			Monochrome,
		}
		[Serialize.Parameter]
		public Colorspace Format { get; set; }
		Collection.IVector<Tuple<Raster.Image, Tuple<string, object>[]>> cache;
		public sealed override int Count { get { return this.cache.IsNull() ? 0 : this.cache.Count; } }
		public sealed override Tuple<Raster.Image, Tuple<string, object>[]> this[int index] { get { return this.cache[index % this.cache.Count]; } }
		protected Cached()
		{
		}
		protected virtual int Prepare(Uri.Locator argument)
		{
			return 0;
		}
		protected virtual Tuple<Raster.Image, Tuple<string, object>[]> Generate(int frame)
		{
			return null;
		}
		public sealed override void Open(Uri.Locator argument)
		{
			this.cache = new Collection.Vector<Tuple<Raster.Image, Tuple<string, object>[]>>(this.Prepare(argument));
			new Action<int>(frame =>
			{
				Tuple<Raster.Image, Tuple<string, object>[]> result = this.Generate(frame);
				Raster.Image generated = result.Item1;
				{
					switch (this.Format)
					{
						default:
						case Colorspace.Bgra:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Bgra>(), result.Item2);
							break;
						case Colorspace.Bgr:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Bgr>(), result.Item2);
							break;
						case Colorspace.Yuv420:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Yuv420>(), result.Item2);
							break;
						case Colorspace.Yvu420:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Yvu420>(), result.Item2);
							break;
						case Colorspace.Yuyv:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Yuyv>(), result.Item2);
							break;
						case Colorspace.Monochrome:
							result = Tuple.Create<Raster.Image, Tuple<string, object>[]>(generated.Convert<Raster.Monochrome>(), result.Item2);
							break;
					}
				}
				lock (this.cache)
					this.cache[frame] = result;
			}).For(this.Count);
		}
		public sealed override void Close()
		{
			lock (this.cache)
				if (this.cache.NotNull())
				{
					this.cache.Apply(frame => frame.Item1.Dispose());
					this.cache = null;
				}
		}
	}
}
