// 
//  Stream.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2012-2013 Imint AB
// 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//  the documentation and/or other materials provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.

using System;
using Kean;
using Kean.Extension;
using Raster = Kean.Draw.Raster;
using Uri = Kean.Uri;
using Geometry2D = Kean.Math.Geometry2D;
using S2253 = Sensoray.S2253;
using Buffer = Kean.Buffer;
using Error = Kean.Error;

namespace Imint.Media.Sensoray.Player
{
	public class Stream :
		Media.Player.IStream,
		Media.Player.ICapture
	{
		public System.Collections.Generic.IEnumerable<Resource> Devices
		{
			get 
			{ 
				Binding.Board board = Binding.Board.Open();
				int count = board.NotNull() ? board.Count : 0;
				for (int i = 0; i < count; i++)
				{
					yield return new Media.Resource(ResourceType.Capture, "Sensoray device " + i + " stream A", "sensoray://" + i + ":0");
					yield return new Media.Resource(ResourceType.Capture, "Sensoray device " + i + " stream B", "sensoray://" + i + ":1");
				}
			}
		}
		public int Channels { get { return 1; } }
		public Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { set; private get; }
		public Status Status { get; private set; }

		Geometry2D.Integer.Size size;
		VideoSystem videoSystem;

		Sensoray.Device device;
		Sensoray.Stream stream;
		Sensoray.Stream recordStream;

		public Stream()
		{ }
		~Stream()
		{
			Error.Log.Wrap((Action)this.Close)();
		}
		public void Poll() { System.Threading.Thread.Sleep(10); }
		public bool Open(Uri.Locator name)
		{
			bool result = false;
			if (name.Scheme == "sensoray")
			{

				int device = 0;
				int stream = 0;
				if (name.Authority.NotNull() && name.Authority.Endpoint.Host.NotNull() && name.Authority.Endpoint.Host.Head.NotEmpty())
				{
					if (name.Authority.Endpoint.Host.Tail.NotNull() && name.Authority.Endpoint.Host.Tail.Head.NotEmpty())
					{

						if (!(int.TryParse(name.Authority.Endpoint.Host.Head, out device)))
							device = 0;
						if (!(int.TryParse(name.Authority.Endpoint.Host.Tail.Head, out stream)))
							stream = 0;
					}
					else if (!(int.TryParse(name.Authority.Endpoint.Host.Head, out stream)))
						stream = 0;
				}
				string system = name.Query["system"];
				this.videoSystem = name.Query.GetEnumeration<VideoSystem>("system", VideoSystem.Pal);
				this.device = Device.Open(device, this.videoSystem);
				this.device.Deinterlace = name.Query.NotFalse("deinterlace");

				this.size = (Geometry2D.Integer.Size)name.Query["size"];
				if (this.size.IsNull() || this.size.Area <= 0)
					this.size = this.device.NativeSize;

				this.stream = this.device[stream];

				string record = null;
				if ((record = name.Query["record"]).NotEmpty())
					result = this.stream.Start(this.size, name.Query.GetEnumeration<Encoding>("encoding", Encoding.H264), name.Query.Get("bitrate", 2500), record);
				else
				{
					result = this.stream.Start(this.size, image => this.Send(0, DateTime.Now, TimeSpan.FromSeconds(this.videoSystem == VideoSystem.Ntsc ? 1 / 30.0f : 1 / 25.0f), image, null));
					string video = null;
					if (result && (video = name.Query["video"]).NotEmpty())
					{
						this.recordStream = this.device[(stream + 1) % 2];
						this.recordStream.Start(this.size, name.Query.GetEnumeration<Encoding>("encoding", Encoding.H264), name.Query.Get("bitrate", 2500), new Uri.Locator("file", video));
					}
				}
				this.Status = result ? Status.Playing : Status.Closed;
			}
			return result;
		}
		public void Close()
		{
			if (this.stream.NotNull())
			{
				this.stream.Stop();
				this.stream = null;
			}
			if (this.recordStream.NotNull())
			{
				this.recordStream.Stop();
				this.recordStream = null;
			}
		}
		void IDisposable.Dispose()
		{
			this.Close();
		}

	}

}
