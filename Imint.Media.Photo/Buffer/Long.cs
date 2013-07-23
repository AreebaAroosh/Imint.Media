using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collection = Kean.Core.Collection;
using Raster = Kean.Draw.Raster;
using Parallel = Kean.Core.Parallel;
using Kean.Core.Collection.Extension;
using Kean.Core.Extension;

namespace Imint.Media.Photo.Buffer
{
	public class Long :
		Abstract
	{
		Collection.IQueue<Raster.Image> buffer;
		int lastIndex = 0;
		Kean.Core.Parallel.RepeatThread loader;
		
		public Long(string[] photoPaths)
		{
			wrap = false;
			buffer = new Collection.Synchronized.Queue<Raster.Image>();
			this.photoPaths = photoPaths;
			this.loader = Parallel.RepeatThread.Start("PhotoLoader", () =>
			{
				while (this.lastIndex-this.index < 10)
				{
					this.buffer.Enqueue(Raster.Image.Open(this.photoPaths[lastIndex % this.Count]));
					this.lastIndex++;
					if (this.lastIndex == this.Count + 10)
						this.lastIndex %= this.Count;
				}
				//lock (this.signal)
				//	System.Threading.Monitor.Wait(this.signal, 20);
			});
		}

		public override Tuple<int, Raster.Image> Next()
		{
			index++;
			Tuple<int, Raster.Image> result = new Tuple<int, Raster.Image>(index, buffer.Dequeue() as Raster.Image);
			return result;
		}

		public override void Close()
		{
			if (this.loader.NotNull())
			{
				this.loader.Dispose();
				this.loader = null;
			}
			Raster.Image image;
			while (!buffer.Empty)
			{
				image = buffer.Dequeue();
				if (image.NotNull())
					image.Dispose();
			}
		}
	}
}
