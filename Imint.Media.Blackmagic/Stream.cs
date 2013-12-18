using DeckLinkAPI;
using Kean;
using Kean.Extension;
using System;
using System.Diagnostics;
using Buffer = Kean.Buffer;
using Collection = Kean.Collection;
using Generic = System.Collections.Generic;
using Geometry2D = Kean.Math.Geometry2D;
using Parallel = Kean.Parallel;
using Platform = Kean.Platform;
using Raster = Kean.Draw.Raster;
using Serialize = Kean.Serialize;
using Uri = Kean.Uri;

namespace Imint.Media.Blackmagic
{
	class Stream :
		IDeckLinkInputCallback,
		Media.Player.IStream,
		Media.Player.ICapture,
		Platform.IHasApplication
	{
		IDeckLinkInput deckLinkInput;
		IDeckLinkConfiguration conf;

		[Serialize.Parameter("Preset")]
		public Collection.List<KeyValue<string, Uri.Locator>> Presets { get; private set; }

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
			//deckLinkInput.FlushStreams();
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
				IDeckLink deckLink;
				IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
				int device = 0;
				deckLinkIterator.Next(out deckLink);
				while (int.Parse(name.Authority) > device)
				{
					deckLinkIterator.Next(out deckLink);
					device++;
				}
				if (deckLink.NotNull())
				{
					deckLinkInput = (IDeckLinkInput)deckLink;
					deckLinkInput.SetCallback(this);
		
					long connectionInt;
					this.conf = (IDeckLinkConfiguration)deckLinkInput;
					this.conf.GetInt(_BMDDeckLinkConfigurationID.bmdDeckLinkConfigVideoInputConnection, out connectionInt);
					_BMDVideoConnection connection = new Connection(name.Query, (_BMDVideoConnection)connectionInt).Cable;
					this.conf.SetInt(_BMDDeckLinkConfigurationID.bmdDeckLinkConfigVideoInputConnection, (long)connection);
					this.conf.GetInt(_BMDDeckLinkConfigurationID.bmdDeckLinkConfigVideoInputConnection, out connectionInt);
					connection = new Connection(name.Query, (_BMDVideoConnection)connectionInt).Cable;
					DisplayMode m = new DisplayMode(name.Query);
					PixelFormat p = new PixelFormat(name.Query);
					_BMDDisplayModeSupport modeSupport = _BMDDisplayModeSupport.bmdDisplayModeNotSupported;
					IDeckLinkDisplayMode outMode;
					deckLinkInput.DoesSupportVideoMode(m.Mode, p.Format, _BMDVideoInputFlags.bmdVideoInputFlagDefault, out modeSupport, out outMode);
					if (modeSupport == _BMDDisplayModeSupport.bmdDisplayModeSupported)
					{
						deckLinkInput.EnableVideoInput(m.Mode, p.Format, _BMDVideoInputFlags.bmdVideoInputFlagDefault);
						deckLinkInput.StartStreams();
						this.Status = Media.Status.Playing;
						result = true;
					}
				}
			}
			return (result);
		}

		public void Close()
		{
			if (this.deckLinkInput.NotNull())
			{
				this.deckLinkInput.StopStreams();
				//this.deckLinkInput.FlushStreams();
				this.deckLinkInput.DisableVideoInput();
				this.deckLinkInput = null;
			}
		}

		public Status Status
		{
			get;
			private set;
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
				int device = 0;
				string name;
				{
					IDeckLink deckLink;
					IDeckLinkIterator deckLinkIterator = new CDeckLinkIterator();
					deckLinkIterator.Next(out deckLink);
					while (deckLink.NotNull())
					{
						deckLinkInput = (IDeckLinkInput)deckLink;
						deckLink.GetDisplayName(out name);
						yield return new Media.Resource(ResourceType.Capture, name, "blackmagic://" + device++);
						deckLinkIterator.Next(out deckLink);
					}
					if (device > 0)
					{
						foreach (KeyValue<string, Uri.Locator> preset in this.Presets)
							yield return new Media.Resource(ResourceType.Capture, preset.Key, preset.Value);
					}
				}
			}
		}
	}
}
