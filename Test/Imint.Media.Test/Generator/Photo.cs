// 
//  Photo.cs
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
using Bitmap = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Uri;
using Interpolation = Kean.Math.Regression.Interpolation;
using Collection = Kean.Collection;
using Kean.Extension;
using Kean.Collection.Extension;
using Kean.Collection.Linked.Extension;

namespace Imint.Media.Test.Generator
{
	public class Photo :
		Cached
	{
		enum MotionType
		{
			Repeat,
			Mirror,
			Periodic,
		}
		class ControlPoint
		{
			public float Angle { get; set; }
			public float Scale { get; set; }
			public float X { get; set; }
			public float Y { get; set; }
			public ControlPoint() { }
			public ControlPoint(float angle, float scale, float x, float y)
			{
				this.Angle = angle;
				this.Scale = scale;
				this.X = x;
				this.Y = y;
			}
		}
		bool metaData;
		int frames;
		Geometry2D.Integer.Size resolution;
		Collection.IList<ControlPoint> controlPoints;
		MotionType motionType;
		public Photo()
		{
			this.Initialize();
		}
		Interpolation.Splines.Geometry2D.Single.Transform interpolate;
		Geometry2D.Single.Transform[] motion;
		Bitmap.Image photo;
		public override string Name
		{
			get { return "photo"; }
		}
		protected override int Prepare(Uri.Locator argument)
		{

			try
			{
				Uri.Path path = argument.PlatformPath;
				if (path.NotNull())
				{
					Bitmap.Image image = Bitmap.Image.Open(path);
					this.photo = image;
				}
				if (!argument.Query.Empty)
				{
					string value = argument.Query["resolution"];
					if (value.NotEmpty())
						this.resolution = (Geometry2D.Integer.Size)value;
					value = argument.Query["format"];
					if (value.NotEmpty())
						base.Format = (Colorspace)Enum.Parse(typeof(Colorspace), value, true);
					value = argument.Query["frames"];
					if (value.NotEmpty())
						this.frames = Kean.Math.Integer.Parse(value);
					value = argument.Query["meta"];
					if (value.NotEmpty())
						this.metaData = bool.Parse(value);
					value = argument.Query["motiontype"];
					if (value.NotEmpty())
						this.motionType = (MotionType)Enum.Parse(typeof(MotionType), value, true);
					value = argument.Query["motion"];
					if (value.NotEmpty())
					{
						string[] motion = value.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
						this.controlPoints.Clear();
						switch (this.motionType)
						{
							default:
							case MotionType.Repeat:
							case MotionType.Periodic:
								for (int i = 0; i < motion.Length - 3; i += 4)
									this.controlPoints.Add(new ControlPoint(Kean.Math.Single.Parse(motion[i]), Kean.Math.Single.Parse(motion[i + 1]), Kean.Math.Single.Parse(motion[i + 2]), Kean.Math.Single.Parse(motion[i + 3])));
								break;
							case MotionType.Mirror:
								for (int i = 0; i < motion.Length - 3; i += 4)
									this.controlPoints.Add(new ControlPoint(Kean.Math.Single.Parse(motion[i]), Kean.Math.Single.Parse(motion[i + 1]), Kean.Math.Single.Parse(motion[i + 2]), Kean.Math.Single.Parse(motion[i + 3])));
								for (int i = motion.Length - 5; i >= 3; i -= 4)
									this.controlPoints.Add(new ControlPoint(Kean.Math.Single.Parse(motion[i - 3]), Kean.Math.Single.Parse(motion[i - 2]), Kean.Math.Single.Parse(motion[i - 1]), Kean.Math.Single.Parse(motion[i])));
								break;
						}
					}

				}
			}
			catch (Exception)
			{
				this.Initialize();
			}
			this.motion = new Geometry2D.Single.Transform[this.frames];
			float step = this.frames / (float)(this.controlPoints.Count - 1);
			Tuple<float, Geometry2D.Single.Transform>[] measures = new Tuple<float, Geometry2D.Single.Transform>[this.controlPoints.Count];
			for (int i = 0; i < this.controlPoints.Count; i++)
				measures[i] = Tuple.Create<float,Geometry2D.Single.Transform>(i * step, (Geometry2D.Single.Transform)(Geometry2D.Single.Transform.CreateTranslation(this.controlPoints[i].X, this.controlPoints[i].Y) * Geometry2D.Single.Transform.CreateRotation(Kean.Math.Single.ToRadians(this.controlPoints[i].Angle)) * Geometry2D.Single.Transform.CreateScaling(this.controlPoints[i].Scale)));
			Interpolation.Splines.Method method;
			switch (this.motionType)
			{
				default:
				case MotionType.Repeat:
					method = Interpolation.Splines.Method.Natural;
					break;
				case MotionType.Periodic:
					method = Interpolation.Splines.Method.Periodic;
					break;
				case MotionType.Mirror:
					method = Interpolation.Splines.Method.Natural;
					break;
				
			}
			this.interpolate = new Kean.Math.Regression.Interpolation.Splines.Geometry2D.Single.Transform(method, measures);
			return this.frames;
		}
		protected override Tuple<Bitmap.Image, Tuple<string, object>[]> Generate(int frame)
		{
			Geometry2D.Single.Transform initialValues =this.interpolate.Interpolate(0);
			Geometry2D.Single.Transform initialAbsolute = initialValues;
			Geometry2D.Single.Transform currentValues = this.interpolate.Interpolate(frame);
			Geometry2D.Single.Transform currentAbsolute = currentValues;
			Tuple<string, object>[] meta = null;
			if (this.metaData)
			{
				int previousFrame = frame > 0 ? frame - 1 : (this.motionType == MotionType.Mirror ? this.frames - 1 : 0);
				Geometry2D.Single.Transform previousValues = this.interpolate.Interpolate(previousFrame);
				Geometry2D.Single.Transform previousAbsolute = previousValues;
				meta = new Tuple<string, object>[2] { Tuple.Create<string, object>("RelativeSyntetic", previousAbsolute.Inverse * currentAbsolute), Tuple.Create<string, object>("AbsoluteSyntetic", initialAbsolute.Inverse * currentAbsolute) };
			}
			return Tuple.Create<Bitmap.Image, Tuple<string, object>[]>(this.photo.Copy(this.resolution, currentAbsolute) as Bitmap.Image, meta);
		}
		private void Initialize()
		{
			this.metaData = false;
			this.frames = 25 * 2;
			this.resolution = new Geometry2D.Integer.Size(640, 480);
			this.photo = Bitmap.Image.OpenResource("Generator/strip.png");
			this.Format = Colorspace.Yuv420;
			this.motionType = MotionType.Mirror;
			this.controlPoints = new Collection.List<ControlPoint>();
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 40, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, -40, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
		}
	}
}
