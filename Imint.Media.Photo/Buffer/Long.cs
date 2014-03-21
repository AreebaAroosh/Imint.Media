using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Collection = Kean.Collection;
using Raster = Kean.Draw.Raster;
using Parallel = Kean.Parallel;
using Kean.Collection.Extension;
using Kean.Extension;

namespace Imint.Media.Photo.Buffer
{
	class Long :
		Abstract
	{
		int tailIndex;
		Collection.IQueue<Raster.Image> buffer;
		Kean.Parallel.RepeatThread loader;
		object signal = new object();
		int? seek;
		
		public Long(string[] photoPaths)
		{
			this.buffer = new Collection.Synchronized.Queue<Raster.Image>();
			this.PhotoPaths = photoPaths;
			this.Wrap = (this.Count <= 1000);
			// If the number of photos is larger than some arbitrary value, playback should not loop.
			this.loader = Parallel.RepeatThread.Start("PhotoLoader", this.Wrap ? (Action)this.WrappingLoader : this.Loader);
		}
		void Loader()
		{
			lock (this.signal)
			{
				if (this.seek.NotNull() && base.Seek((int)this.seek))
				{
					this.seek = null;
					this.tailIndex = this.Position;
					this.buffer.Clear();
				}
				while (this.tailIndex - this.Position < 10 && this.tailIndex < this.Count)
					this.buffer.Enqueue(Raster.Image.Open(this.PhotoPaths[this.tailIndex++]));
			}
			lock (this.signal)
				System.Threading.Monitor.Wait(this.signal, 20);
		}
		void WrappingLoader()
		{
			lock (this.signal)
			{
				if (this.seek.NotNull() && base.Seek((int)this.seek))
				{
					this.seek = null;
					this.tailIndex = this.Position;
					this.buffer.Clear();
				}
				while (this.tailIndex - this.Position < 10)
					this.buffer.Enqueue(Raster.Image.Open(this.PhotoPaths[this.tailIndex++ % this.Count]));
				if (this.Position < 10)
					this.tailIndex %= this.Count;
			}
			lock (this.signal)
				System.Threading.Monitor.Wait(this.signal, 20);
		}
		public override Tuple<int, Raster.Image> Next()
		{
			lock (this.signal)
			{
				if (this.tailIndex - this.Position < 10)
					System.Threading.Monitor.Pulse(this.signal);
			}
			this.Position++;
			if (this.Wrap)
				this.Position %= this.Count;
			return Tuple.Create(this.Position, this.buffer.Dequeue() as Raster.Image);
		}
		public override bool Seek(int position)
		{
			bool result;
			lock (this.signal)
			{
				this.seek = position;
				result = true;
				System.Threading.Monitor.Pulse(this.signal);
			}
			return result;
		}
		public override void Close()
		{
			if (this.loader.NotNull())
			{
				this.loader.Stop();
				this.loader.Abort();
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
