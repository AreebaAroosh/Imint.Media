using System;
using Kean;
using Kean.Extension;
using Collection = Kean.Collection;
using Kean.Collection.Extension;
using Uri = Kean.Uri;
using Algebra = Kean.Math.Algebra;
using Geometry2D = Kean.Math.Geometry2D;
using Geometry3D = Kean.Math.Geometry3D;
using Generic = System.Collections.Generic;

namespace Imint.Media.Test.Generator.Motion
{
	public abstract class Abstract
	{
		public MotionType MotionType { get; set; }
		protected Abstract()
		{
		}
		public abstract Generic.IEnumerable<Geometry2D.Single.Transform> Get2DTransforms(int count);
		public abstract Generic.IEnumerable<Geometry3D.Single.Transform> Get3DTransforms(int count);
		public static Abstract Parse(Uri.Query query)
		{
			MotionType motionType = query.GetEnumeration<MotionType>("motiontype", MotionType.Mirror);
			string motion = query["motion"];
			return motion.NotEmpty() ?
				(Abstract)Interpolation.Parse(motionType, motion) :
				new Function {
				X = (Kean.Math.Algebra.Expression)(query["x"] ?? "sin(t * 3.14) * 200"),
				Y = (Kean.Math.Algebra.Expression)query["y"],
				Z = (Kean.Math.Algebra.Expression)(query["z"] ?? query["s"] ?? query["scale"]),
				RotationX = (Kean.Math.Algebra.Expression)(query["rx"] ?? query["rotationX"]),
				RotationY = (Kean.Math.Algebra.Expression)(query["ry"] ?? query["rotationY"]),
				RotationZ = (Kean.Math.Algebra.Expression)(query["rz"] ?? query["rotationZ"] ?? query["r"] ?? query["rotation"]),
				MotionType = motionType
			};
		}
	}
}

