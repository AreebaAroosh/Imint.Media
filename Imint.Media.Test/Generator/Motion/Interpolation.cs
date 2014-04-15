using System;
using Kean;
using Raster = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Geometry3D = Kean.Math.Geometry3D;
using Uri = Kean.Uri;
using Regression = Kean.Math.Regression;
using Collection = Kean.Collection;
using Kean.Extension;
using Kean.Collection.Extension;
using Kean.Collection.Linked.Extension;
using Integer = Kean.Math.Integer;
using Generic = System.Collections.Generic;

namespace Imint.Media.Test.Generator.Motion
{
	public class Interpolation :
	Abstract
	{
		class ControlPoint
		{
			public float Angle { get; set; }
			public float Scale { get; set; }
			public float X { get; set; }
			public float Y { get; set; }
			public ControlPoint()
			{
			}
			public ControlPoint(float angle, float scale, float x, float y)
			{
				this.Angle = angle;
				this.Scale = scale;
				this.X = x;
				this.Y = y;
			}
		}
		Collection.IList<ControlPoint> controlPoints;
		Interpolation()
		{
			this.MotionType = MotionType.Mirror;
			this.controlPoints = new Collection.List<ControlPoint>();
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 40, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, -40, 0));
			this.controlPoints.Add(new ControlPoint(0, 1, 0, 0));
		}
		public override Generic.IEnumerable<Geometry2D.Single.Transform> Get2DTransforms(int count)
		{
			float step = count / (float)(this.controlPoints.Count - 1);
			var measures = new Tuple<float, Geometry2D.Single.Transform>[this.controlPoints.Count];
			for (int i = 0; i < this.controlPoints.Count; i++)
				measures[i] = Tuple.Create<float,Geometry2D.Single.Transform>(i * step, (Geometry2D.Single.Transform.CreateTranslation(this.controlPoints[i].X, this.controlPoints[i].Y) * Geometry2D.Single.Transform.CreateRotation(Kean.Math.Single.ToRadians(this.controlPoints[i].Angle)) * Geometry2D.Single.Transform.CreateScaling(this.controlPoints[i].Scale)));
			Regression.Interpolation.Splines.Method method;
			switch (this.MotionType)
			{
				default:
				case MotionType.Repeat:
				case MotionType.Mirror:
					method = Regression.Interpolation.Splines.Method.Natural;
					break;
				case MotionType.Periodic:
					method = Regression.Interpolation.Splines.Method.Periodic;
					break;
			}
			var interpolate = new Regression.Interpolation.Splines.Geometry2D.Single.Transform(method, measures);
			for (int frame = 0; frame < count; frame++)
				yield return interpolate.Interpolate(frame);
		}
		public override Generic.IEnumerable<Geometry3D.Single.Transform> Get3DTransforms(int count)
		{
			foreach (var t in this.Get2DTransforms(count))
				yield return (Geometry3D.Single.Transform)t;
		}
		public static Interpolation Parse(MotionType type, string data)
		{
			string[] motion = data.Split(new char[] { ' ', ',' }, StringSplitOptions.RemoveEmptyEntries);
			Collection.IList<ControlPoint> controlPoints = new Collection.List<ControlPoint>();
			switch (type)
			{
				default:
				case MotionType.Repeat:
				case MotionType.Periodic:
					for (int i = 0; i < motion.Length - 3; i += 4)
						controlPoints.Add(new ControlPoint(Single.Parse(motion[i]), Single.Parse(motion[i + 1]), Single.Parse(motion[i + 2]), Single.Parse(motion[i + 3])));
					break;
				case MotionType.Mirror:
					for (int i = 0; i < motion.Length - 3; i += 4)
						controlPoints.Add(new ControlPoint(Single.Parse(motion[i]), Single.Parse(motion[i + 1]), Single.Parse(motion[i + 2]), Single.Parse(motion[i + 3])));
					for (int i = motion.Length - 5; i >= 3; i -= 4)
						controlPoints.Add(new ControlPoint(Single.Parse(motion[i - 3]), Single.Parse(motion[i - 2]), Single.Parse(motion[i - 1]), Single.Parse(motion[i])));
					break;
			}
			return new Interpolation() { controlPoints = controlPoints, MotionType = type };
		}
	}
}

