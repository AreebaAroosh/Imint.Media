using System;
using Kean;
using Kean.Extension;
using Collection = Kean.Collection;
using Kean.Collection.Extension;
using Uri = Kean.Uri;
using Algebra = Kean.Algebra;
using Geometry2D = Kean.Math.Geometry2D;
using Geometry3D = Kean.Math.Geometry3D;
using Generic = System.Collections.Generic;

namespace Imint.Media.Test
{
	public class Motion
	{
		public Geometry2D.Single.Size FieldOfView { get; set; }
		public Algebra.Expression X { get; set; }
		public Algebra.Expression Y { get; set; }
		public Algebra.Expression Z { get; set; }
		public Algebra.Expression RotationX { get; set; }
		public Algebra.Expression RotationY { get; set; }
		public Algebra.Expression RotationZ { get; set; }
		public Motion()
		{
		}
		public Generic.IEnumerable<Geometry2D.Single.Transform> Get2DTransforms(int count)
		{
			float delta = 1f / count;
			for (float time = 0; time <= 1.0f; time += delta)
			{
				Geometry2D.Single.Transform result = Geometry2D.Single.Transform.CreateTranslation(
					                                     this.X.NotNull() ? this.X.Evaluate(KeyValue.Create("t", time)) : 0,
					                                     this.Y.NotNull() ? this.Y.Evaluate(KeyValue.Create("t", time)) : 0);
				if (this.Z.NotNull()) // TODO: use field of view
					result *= Geometry2D.Single.Transform.CreateScaling(this.Z.Evaluate(KeyValue.Create("t", time)));
				if (this.RotationZ.NotNull())
					result *= Geometry2D.Single.Transform.CreateRotation(this.RotationZ.Evaluate(KeyValue.Create("t", time)));
				yield return result;
			}
		}
		public Generic.IEnumerable<Geometry3D.Single.Transform> Get3DTransforms(int count)
		{
			float delta = 1f / count;
			for (float time = 0; time <= 1.0f; time += delta)
			{
				Geometry3D.Single.Transform result = Geometry3D.Single.Transform.CreateTranslation(
					                                     this.X.NotNull() ? this.X.Evaluate(KeyValue.Create("t", time)) : 0,
					                                     this.Y.NotNull() ? this.Y.Evaluate(KeyValue.Create("t", time)) : 0,
					                                     this.Z.NotNull() ? this.Y.Evaluate(KeyValue.Create("t", time)) : 0);
				if (this.RotationX.NotNull())
					result *= Geometry3D.Single.Transform.CreateRotationX(this.RotationX.Evaluate(KeyValue.Create("t", time)));
				if (this.RotationY.NotNull())
					result *= Geometry3D.Single.Transform.CreateRotationY(this.RotationY.Evaluate(KeyValue.Create("t", time)));
				if (this.RotationZ.NotNull())
					result *= Geometry3D.Single.Transform.CreateRotationZ(this.RotationZ.Evaluate(KeyValue.Create("t", time)));
				yield return result;
			}
		}
		public static Motion Parse(Uri.Query query)
		{
			return new Motion {
				X = query["x"],
				Y = query["y"],
				Z = query["z"] ?? query["s"] ?? query["scale"],
				RotationX = query["rx"] ?? query["rotationX"],
				RotationY = query["ry"] ?? query["rotationY"],
				RotationZ = query["rz"] ?? query["rotationZ"] ?? query["r"] ?? query["rotation"],
			};
		}
	}
}

