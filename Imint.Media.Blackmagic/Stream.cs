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
		float rate;

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
		void IDeckLinkInputCallback.VideoInputFrameArrived(IDeckLinkVideoInputFrame videoFrame, IDeckLinkAudioInputPacket audioPacket)
		{
			IntPtr pointer;
			videoFrame.GetBytes(out pointer);
			this.stride = videoFrame.GetRowBytes();
			_BMDPixelFormat format = videoFrame.GetPixelFormat();
			this.size = new Geometry2D.Integer.Size(videoFrame.GetWidth(), videoFrame.GetHeight());
			var buffer = new Buffer.Sized(pointer, this.stride * this.size.Height).Copy();
			Raster.Image frame;
			switch (format)
			{
				case _BMDPixelFormat.bmdFormat8BitYUV:
					frame = new Raster.Uyvy(buffer, size);
					break;
				case _BMDPixelFormat.bmdFormat8BitARGB:
				case _BMDPixelFormat.bmdFormat8BitBGRA:
					frame = new Raster.Bgra(buffer, size);
					break;
				default:
					frame = null;
					break;
			}
			if (frame.NotNull())
				this.threadPool.Enqueue(() =>
					this.Send(0, DateTime.Now, TimeSpan.FromSeconds(1 / this.rate), frame, null));
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
					this.rate = m.Rate;
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
				this.conf = null;
				//this.threadPool.Abort();
				//this.threadPool.TryDispose();
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
			this.Status = Media.Status.Closed;
			this.Close();
		}

		public Generic.IEnumerable<Resource> Devices
		{
			get
			{
				Generic.IEnumerator<Resource> devices = this.EnumerateDevices().GetEnumerator();
				Media.Resource result;
				while (true)
				{
					try
					{
						if (!devices.MoveNext())
							break;
						result = devices.Current;
					}
					catch (System.Runtime.InteropServices.COMException)
					{
						result = null;
					}
					if (result.NotNull())
						yield return result;
				}
			}
		}

		private Generic.IEnumerable<Resource> EnumerateDevices()
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
					bool hasPresets = false;
					foreach (KeyValue<string, Uri.Locator> preset in this.Presets.Where(p => int.Parse(p.Value.Authority) == device))
					{
						hasPresets = true;
						yield return new Media.Resource(ResourceType.Capture, preset.Key, preset.Value);
					}
					if (!hasPresets)
						yield return new Media.Resource(ResourceType.Capture, name, "blackmagic://" + device);
					device++;
					deckLinkIterator.Next(out deckLink);
				}
			}
		}
	}
}
