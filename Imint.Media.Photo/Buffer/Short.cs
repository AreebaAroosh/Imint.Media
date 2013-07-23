using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collection = Kean.Core.Collection;
using Raster = Kean.Draw.Raster;

namespace Imint.Media.Photo.Buffer
{
	class Short :
		Abstract
	{
		Raster.Image[] buffer;
		public Short(string[] photoPaths)
		{
			this.Wrap = true;
			this.PhotoPaths = photoPaths;
			this.buffer = new Raster.Image[this.Count];
			for (int i = 0; i < this.Count; i++)
				this.buffer[i] = Raster.Image.Open(photoPaths[i]);
		}

		public override Tuple<int, Raster.Image> Next()
		{
			Tuple<int, Raster.Image> result = new Tuple<int, Raster.Image>(this.Position, this.buffer[this.Position++].Copy() as Raster.Image);
			if (this.Wrap)
				this.Position %= this.Count;
			return result;
		}
		public override void Close()
		{
			for (int i = 0; i < this.Count; i++)
			{
				this.buffer[i].Dispose();
				this.buffer[i] = null;
			}
		}
	}
}
