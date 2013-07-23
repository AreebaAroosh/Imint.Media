using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collection = Kean.Core.Collection;
using Raster = Kean.Draw.Raster;

namespace Imint.Media.Photo.Buffer
{
	public class Short :
		Abstract
	{
		Raster.Image[] buffer;
		public Short(string[] photoPaths)
		{
			wrap = true;
			this.photoPaths = photoPaths;
			buffer = new Raster.Image[this.Count];
			for (int i = 0; i < this.Count; i++)
			{
				buffer[i] = Raster.Image.Open(photoPaths[i]);
			}
		}

		public override Tuple<int, Raster.Image> Next()
		{
			Tuple<int, Raster.Image> result = new Tuple<int, Raster.Image>(index, buffer[index++].Copy() as Raster.Image);
			index %= this.Count;
			return result;
		}

		public override void Close()
		{
			for (int i = 0; i < this.Count; i++)
			{
				buffer[i].Dispose();
				buffer[i] = null;
			}
		}
	}
}
