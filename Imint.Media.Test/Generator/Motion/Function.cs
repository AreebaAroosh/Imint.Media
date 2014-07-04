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
	public class Function :
	Abstract
	{
		public Geometry2D.Single.Size FieldOfView { get; set; }
		public Kean.Math.Algebra.Expression X { get; set; }
		public Kean.Math.Algebra.Expression Y { get; set; }
		public Kean.Math.Algebra.Expression Z { get; set; }
		public Kean.Math.Algebra.Expression RotationX { get; set; }
		public Kean.Math.Algebra.Expression RotationY { get; set; }
		public Kean.Math.Algebra.Expression RotationZ { get; set; }
		public Function()
		{
		}
        public override Generic.IEnumerable<Geometry2D.Single.Transform> Get2DTransforms(int count)
        {
            float delta = 1f / count;
            for (float time = 0; time < 1.0f; time += delta)
            {
                Geometry2D.Single.Transform result = Geometry2D.Single.Transform.CameraTo2DTransform(
                    new Kean.Math.Geometry3D.Single.EuclidTransform(
                        new Kean.Math.Geometry3D.Single.Rotation(this.RotationX.NotNull() ? this.RotationX.Evaluate(KeyValue.Create("t", time)) : 0,
                                                                 this.RotationY.NotNull() ? this.RotationY.Evaluate(KeyValue.Create("t", time)) : 0,
                                                                 this.RotationZ.NotNull() ? this.RotationZ.Evaluate(KeyValue.Create("t", time)) : 0),
                        new Geometry3D.Single.Size(this.X.NotNull() ? this.X.Evaluate(KeyValue.Create("t", time)) : 0,
                                                   this.Y.NotNull() ? this.Y.Evaluate(KeyValue.Create("t", time)) : 0,
                                                   this.Z.NotNull() ? this.Z.Evaluate(KeyValue.Create("t", time)) : 0)),
                        this.FieldOfView,
                        new Geometry2D.Single.Size(640, 480));
                yield return result;
            }
        }
		public override Generic.IEnumerable<Geometry3D.Single.Transform> Get3DTransforms(int count)
		{
			float delta = 1f / count;
			for (float time = 0; time < 1.0f; time += delta)
			{
				var result = Geometry3D.Single.Transform.Identity;
				if (this.RotationX.NotNull())
					result = result.RotateX(this.RotationX.Evaluate(KeyValue.Create("t", time)));
				if (this.RotationY.NotNull())
					result = result.RotateY(this.RotationY.Evaluate(KeyValue.Create("t", time)));
				if (this.RotationZ.NotNull())
					result = result.RotateZ(this.RotationZ.Evaluate(KeyValue.Create("t", time)));
				result = result.Translate(new Geometry3D.Single.Size(
					                  this.X.NotNull() ? this.X.Evaluate(KeyValue.Create("t", time)) : 0,
					                  this.Y.NotNull() ? this.Y.Evaluate(KeyValue.Create("t", time)) : 0,
					                  this.Z.NotNull() ? this.Z.Evaluate(KeyValue.Create("t", time)) : 0));
				yield return result;
			}
		}
	}
}

