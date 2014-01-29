using System;
using Raster = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Uri;

namespace Imint.Media
{
	public interface IRecorder
	{
		Status Status { get; }
		event Action<Status> StatusChanged;
		bool Open(Uri.Locator resource, Geometry2D.Integer.Size resolution, TimeSpan lifetime);
		bool Close();
	}
}

