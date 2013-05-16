// 
//  Device.cs
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
using Kean.Core;
using Kean.Core.Extension;
using Geometry2D = Kean.Math.Geometry2D;
using Raster = Kean.Draw.Raster;

namespace Imint.Media.Sensoray
{
	public class Device
	{
		Binding.Device backend;
		public bool Deinterlace 
		{ 
			set { this.backend.Deinterlace = value; } 
			get { return this.backend.Deinterlace; } 
		}
		public int Hue
		{
			set { this.backend.Hue = value; }
			get { return this.backend.Hue; }
		}
		public int Saturation
		{
			set { this.backend.Saturation = value; }
			get { return this.backend.Saturation; }
		}
		public int Contrast
		{
			set { this.backend.Contrast = value; }
			get { return this.backend.Contrast; }
		}
		public int Brightness
		{
			set { this.backend.Brightness = value; }
			get { return this.backend.Brightness; }
		}
		VideoSystem videoSystem;
		public VideoSystem VideoSystem
		{
			private set { this.backend.VideoSystem = this.videoSystem = value; }
			get { return this.videoSystem; }
		}
		public Geometry2D.Integer.Size NativeSize
		{
			get
			{
				Geometry2D.Integer.Size result;
				switch (this.VideoSystem)
				{
					default:
					case VideoSystem.Pal:
						result = new Geometry2D.Integer.Size(704, 576);
						break;
					case VideoSystem.Ntsc:
						result = new Geometry2D.Integer.Size(704, 480);
						break;
				}
				return result;
			}
		}
		Stream[] streams = new Stream[2];
		public Stream this[int stream]
		{
			get
			{
				Stream result = null;
				if (stream >= 0 && stream < 2)
				{
					result = this.streams[stream];
					if (result.IsNull())
					{
						Binding.Stream backendStream = this.backend[stream];
						if (backendStream.NotNull())
							result = this.streams[stream] = new Stream(this, backendStream);
					}
				}
				return result;
			}
		}
		Device(Binding.Device backend)
		{
			this.backend = backend;
		}
		public static Device Open(int device, VideoSystem videoSystem)
		{
			Device result = null;
			Binding.Device backend = Binding.Board.Open()[device];
			if (backend.NotNull())
			{
				backend.VideoSystem = videoSystem;
				result = new Device(backend);
			}
			return result;
		}
	}
}
