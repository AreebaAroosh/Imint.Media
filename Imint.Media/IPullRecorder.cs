using System;
using Raster = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Uri;

namespace Imint.Media
{
	public interface IPullRecorder :
	IRecorder
	{
		Func<Raster.Image, bool> Poll { set; }
		bool Play();
		bool Pause();
	}
}

