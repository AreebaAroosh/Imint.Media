using System;
using DeckLinkAPI;
using System.Runtime.InteropServices;
using Raster = Kean.Draw.Raster;
using Buffer = Kean.Buffer;
using Geometry2D = Kean.Math.Geometry2D;

namespace Imint.Media.Blackmagic
{
	public class Capture :
		IDeckLinkInputCallback
	{
		void IDeckLinkInputCallback.VideoInputFormatChanged(_BMDVideoInputFormatChangedEvents notificationEvents, IDeckLinkDisplayMode newDisplayMode, _BMDDetectedVideoInputFormatFlags detectedSignalFlags)
		{
			throw new NotImplementedException();
		}

		void IDeckLinkInputCallback.VideoInputFrameArrived(IDeckLinkVideoInputFrame videoFrame, IDeckLinkAudioInputPacket audioPacket)
		{
			IntPtr ip = new IntPtr();
			videoFrame.GetBytes(out ip);
			Raster.Image image = new Raster.Yuv420(new Buffer.Sized(ip, videoFrame.GetRowBytes() * videoFrame.GetHeight()),
													new Geometry2D.Integer.Size(videoFrame.GetWidth(), videoFrame.GetHeight()));
		}
	}
}
