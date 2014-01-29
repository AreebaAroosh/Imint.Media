using System;
using Raster = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Uri;

namespace Imint.Media
{
	public interface IPushRecorder :
	IRecorder
	{
		bool Append (Raster.Image frame);
	}
}

