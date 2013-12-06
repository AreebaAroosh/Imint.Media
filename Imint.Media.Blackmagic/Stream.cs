using DeckLinkAPI;
using Kean.Extension;
using System;
using System.Diagnostics;
using Buffer = Kean.Buffer;
using Generic = System.Collections.Generic;
using Geometry2D = Kean.Math.Geometry2D;
using Parallel = Kean.Parallel;
using Platform = Kean.Platform;
using Raster = Kean.Draw.Raster;

namespace Imint.Media.Blackmagic
{
	class Stream :
		IDeckLinkInputCallback,
		Media.Player.IStream,
		Media.Player.ICapture,
		Platform.IHasApplication
	{
		IDeckLink deckLink;
		IDeckLinkInput deckLinkInput;

		Parallel.ThreadPool threadPool;
		Platform.Application application;
		Platform.Application Platform.IHasApplication.Application
		{
			get { return this.application; }
			set
			{
				this.application = value;
				if (this.application.NotNull())
					this.application["ThreadPool"].WhenLoaded<Platform.Module<Parallel.ThreadPool>>(t => this.threadPool = t.Value);
			}
		}
		public Stream()
		{
		}

		void IDeckLinkInputCallback.VideoInputFormatChanged(_BMDVideoInputFormatChangedEvents notificationEvents, IDeckLinkDisplayMode newDisplayMode, _BMDDetectedVideoInputFormatFlags detectedSignalFlags)
		{
			Debug.WriteLine("VideoInputFormatChanged called");
		}
		int stride;
		Geometry2D.Integer.Size size;
		int divisor = 1;
		long count;
		void IDeckLinkInputCallback.VideoInputFrameArrived(IDeckLinkVideoInputFrame videoFrame, IDeckLinkAudioInputPacket audioPacket)
		{
			if (this.count++ % this.divisor == 0)
			{
				IntPtr pointer;
				videoFrame.GetBytes(out pointer);
				this.stride = videoFrame.GetRowBytes();
				this.size = new Geometry2D.Integer.Size(videoFrame.GetWidth(), videoFrame.GetHeight());
				var buffer = new Buffer.Sized(pointer, this.stride * this.size.Height).Copy();
				this.threadPool.Enqueue(() =>
					this.Send(0, DateTime.Now, TimeSpan.FromSeconds(1 / 50.0f * this.divisor), new Raster.Uyvy(buffer, size), null));
			}
		}
	
		public int Channels
		{
			get { return 1; }
		}

		public void Poll()
		{
			System.Threading.Thread.Sleep(10);
		}

		public Action<int, DateTime, TimeSpan, Kean.Draw.Raster.Image, Tuple<string, object>[]> Send { private get; set; }

		public bool Open(Kean.Uri.Locator name)
		{
			bool result = false;
			if (name.Scheme == "blackmagic")
			{
				IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
				deckLinkIterator.Next(out deckLink);
				if (deckLink != null)
				{
					deckLinkInput = (IDeckLinkInput)deckLink;
					deckLinkInput.SetCallback(this);
				}
				//TODO: Hard coded for the Sony HandyCam, needs to be more intelligent.
				deckLinkInput.EnableVideoInput(_BMDDisplayMode.bmdModeHD720p50, _BMDPixelFormat.bmdFormat8BitYUV, _BMDVideoInputFlags.bmdVideoInputFlagDefault);
				deckLinkInput.StartStreams();
				result = true;
				this.Status = Media.Status.Playing;
			}
			return (result);
		}

		public void Close()
		{
			if (this.deckLinkInput.NotNull())
			{
				this.deckLinkInput.StopStreams();
				this.deckLinkInput.DisableVideoInput();
				this.deckLinkInput = null;
			}
		}

		public Status Status
		{
			get; private set;
		}

		public void Dispose()
		{
			this.Close();
			this.Status = Media.Status.Closed;
		}

		public Generic.IEnumerable<Resource> Devices
		{
			get 
			{
				IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
				deckLinkIterator.Next(out deckLink);
				while (deckLink != null)
				{
					deckLinkInput = (IDeckLinkInput)deckLink;
					string name;
					deckLink.GetDisplayName(out name);
					yield return new Media.Resource(ResourceType.Capture, name, "blackmagic://");
					deckLinkIterator.Next(out deckLink);
				}
			}
		}
	}
}
