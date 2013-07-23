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
	class Long :
		Abstract
	{
		int tailIndex = 0;
		Collection.IQueue<Raster.Image> buffer;
		Kean.Core.Parallel.RepeatThread loader;
		object signal = new object();
		
		public Long(string[] photoPaths)
		{
			this.Wrap = false;
			this.buffer = new Collection.Synchronized.Queue<Raster.Image>();
			this.PhotoPaths = photoPaths;
			this.loader = Parallel.RepeatThread.Start("PhotoLoader", () =>
			{
				while (this.tailIndex - this.Position < 10 && this.tailIndex < this.Count)
					this.buffer.Enqueue(Raster.Image.Open(this.PhotoPaths[tailIndex++]));
				lock (this.signal)
					System.Threading.Monitor.Wait(this.signal, 60);
			});
		}

		public override Tuple<int, Raster.Image> Next()
		{
			lock (this.signal)
			{
				if (this.tailIndex - this.Position < 10)
					System.Threading.Monitor.Pulse(this.signal);
			}
			this.Position++;
			Tuple<int, Raster.Image> result = new Tuple<int, Raster.Image>(this.Position, this.buffer.Dequeue() as Raster.Image);
			return result;
		}
		public override bool Seek(int position)
		{
			bool result;
			lock (this.signal)
			{
				this.Clear();
				result = base.Seek(this.tailIndex = position);
				System.Threading.Monitor.Pulse(this.signal);
			}
			return result;
		}
		public override void Close()
		{
			if (this.loader.NotNull())
			{
				this.loader.Dispose();
				this.loader = null;
			}
			this.Clear();
			this.buffer = null;
		}
		void Clear()
		{
			Raster.Image image;
			while (!this.buffer.Empty)
			{
				image = this.buffer.Dequeue();
				if (image.NotNull())
					image.Dispose();
			}
		}
	}
}
