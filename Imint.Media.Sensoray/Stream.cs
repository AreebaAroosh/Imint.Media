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
using Buffer = Kean.Buffer;
using Uri = Kean.Uri;
using Geometry2D = Kean.Math.Geometry2D;
using Raster = Kean.Draw.Raster;

namespace Imint.Media.Sensoray
{
	public class Stream
	{
		Binding.Stream backend;
		Device device;

		internal Stream(Device device, Binding.Stream backend)
		{
			this.device = device;
			this.backend = backend;
		}
		Action<Buffer.Sized> callback;
		public bool Start(Geometry2D.Integer.Size size, Action<Raster.Image> callback)
		{
			this.backend.Type = Encoding.Uyvy;
			this.backend.Size = size;
			this.backend.LowLatencyPreview = size == this.device.NativeSize;
			this.callback = buffer => callback.Call(new Raster.Uyvy(buffer, size));
			this.backend.OnCallback += this.callback;
			return this.backend.StartCallback();
		}
		public bool Start(Geometry2D.Integer.Size size, Encoding encoding, int bitrate, Uri.Locator resource)
		{
			this.backend.Size = size;
			this.backend.Type = encoding;
			this.backend.Mp4Mode = Binding.Mp4Mode.Standard;
			this.backend.RecordMode = Binding.RecordMode.Video;
			this.backend.Bitrate = bitrate;
			return this.backend.StartRecord(resource.PlatformPath);
		}
		public bool Start()
		{
			return this.backend.StartPreview();
		}
		public bool Stop()
		{
			if (this.callback.NotNull())
			{
				this.backend.OnCallback -= this.callback;
				this.callback = null;
			}
			return this.backend.Stop();
		}
	}
}
