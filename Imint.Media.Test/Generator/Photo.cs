// 
//  Photo.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2010-2014 Imint AB
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
using OpenGL = Kean.Draw.OpenGL;
using Geometry2D = Kean.Math.Geometry2D;
using Geometry3D = Kean.Math.Geometry3D;
using Uri = Kean.Uri;
using Interpolation = Kean.Math.Regression.Interpolation;
using Collection = Kean.Collection;
using Kean.Extension;
using Kean.Collection.Extension;
using Kean.Collection.Linked.Extension;
using Integer = Kean.Math.Integer;

namespace Imint.Media.Test.Generator
{
	public class Photo :
		Cached
	{
		bool metaData;
		int frames;
		Geometry2D.Integer.Size size;
		public Photo()
		{
			this.Initialize();
		}
		Motion.Abstract motion;
		Geometry3D.Single.Transform[] transforms;
		Raster.Image photo;
		public override string Name
		{
			get { return "photo"; }
		}
		protected override int Prepare(Uri.Locator argument)
		{
			string platformPath = argument.PlatformPath;
			this.photo = (platformPath.NotEmpty() ? Raster.Image.Open(argument.PlatformPath) : null) ?? Raster.Image.OpenResource("Generator/strip.jpg");
			if (!argument.Query.Empty)
			{
				string value = argument.Query["size"];
				if (value.NotEmpty())
					this.size = (Geometry2D.Integer.Size)value;
				value = argument.Query["format"];
				if (value.NotEmpty())
					this.Format = (Colorspace)Enum.Parse(typeof(Colorspace), value, true);
				value = argument.Query["frames"];
				if (value.NotEmpty())
					this.frames = Integer.Parse(value);
				value = argument.Query["meta"];
				if (value.NotEmpty())
					this.metaData = bool.Parse(value);
			}
			this.motion = Motion.Abstract.Parse(argument.Query);
			this.transforms = this.motion.Get3DTransforms(this.frames).ToArray();
			return this.frames;
		}
		protected override Tuple<Raster.Image, Tuple<string, object>[]> Generate(int frame)
		{
			Geometry3D.Single.Transform initialAbsolute = this.transforms[0];
			Geometry3D.Single.Transform currentAbsolute = this.transforms[frame];
			Tuple<string, object>[] meta = null;
			if (this.metaData)
			{
				int previousFrame = frame > 0 ? frame - 1 : (this.motion.MotionType == Motion.MotionType.Mirror ? this.frames - 1 : 0);
				Geometry3D.Single.Transform previousAbsolute = this.transforms[previousFrame];
				meta = new Tuple<string, object>[2] {
					Tuple.Create<string, object>("RelativeSyntetic", previousAbsolute.Inverse * currentAbsolute),
					Tuple.Create<string, object>("AbsoluteSyntetic", initialAbsolute.Inverse * currentAbsolute)
				};
			}
			var image = new Raster.Bgra(size);

				image.ProjectionOf(this.photo, currentAbsolute, new Geometry2D.Single.Size(45f, 45f));
				return Tuple.Create<Raster.Image, Tuple<string, object>[]>(image as Raster.Image, meta);

			//(this.photo.Copy(this.resolution, currentAbsolute).ResizeTo(this.resolution-
			//	new Geometry2D.Integer.Size(
			//		Kean.Math.Integer.Minimum((frame%4 == 0 ? 1 : 0)*frame*12, this.resolution.Width-1),Kean.Math.Integer.Minimum((frame%4 == 0 ? 1 : 0)*frame*12, this.resolution.Height-1))
			//	).ResizeTo(this.resolution)
			//as Raster.Image, meta);
		}
		void Initialize()
		{
			this.metaData = false;
			this.frames = 25 * 2;
			this.size = new Geometry2D.Integer.Size(640, 480);
			this.Format = Colorspace.Yuv420;
		}
	}
}
