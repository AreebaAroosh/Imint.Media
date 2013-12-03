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

namespace Imint.Media.Sensoray.Binding
{
	public class Stream
	{
		public Device Device { get; private set; }
		int device;
		int stream;
		public int FirmwareVersion
		{
			get
			{
				int result = 0;
				S2253.GetParam(S2253.MID2253_PARAM.MID2253_PARAM_FIRMWARE, ref result, this.device, this.stream);
				return result;
			}
		}
		public Encoding Type
		{
			set { S2253.SetStreamType((int)value, this.device, this.stream); }
		}
		public Mp4Mode Mp4Mode
		{
			set { S2253.SetMp4Mode((S2253.MID2253_MP4MODE)value, this.device, this.stream); }
		}
		public RecordMode RecordMode
		{
			get
			{
				int result = 0;
				S2253.GetRecordMode(ref result, this.device, this.stream);
				return (RecordMode)result;
			}
			set { S2253.SetRecordMode((S2253.MID2253_RECMODE)value, this.device, this.stream); }
		}
		public bool LowLatencyPreview
		{
			set { S2253.LowLatencyPreview(value, this.device, this.stream); }
		}
		public int Bitrate
		{
			set { S2253.SetBitrate(value, this.device, this.stream); }
		}
		public int JpegQuality
		{
			set { S2253.SetJpegQ(value, this.device, this.stream); }
		}
		public Geometry2D.Integer.Size Size
		{
			set { S2253.SetImageSize(value.Width, value.Height, this.device, this.stream); }
		}
		bool callbackRegistred;
		Action<Buffer.Sized> onCallback;
		public event Action<Buffer.Sized> OnCallback
		{
			add
			{
				if (!this.callbackRegistred)
					this.callbackRegistred = S2253.RegisterCallback(this.Callback, this.device, this.stream) == 0;
				this.onCallback += value;
			}
			remove { this.onCallback -= value; }
		}
		Stream(Device device, int stream)
		{
			this.Device = device;
			this.device = device.device;
			this.stream = stream;
		}
		int Callback(IntPtr data, int size, int device, int stream)
		{
			this.onCallback.Call(new Buffer.Sized(data, size));
			return 0;
		}
		public bool StartPreview()
		{
			return S2253.StartPreview(this.device, this.stream) == 0;
		}
		public bool StartCallback()
		{
			return S2253.StartCallback(this.device, this.stream) == 0;
		}
		public bool StartRecord(string filename)
		{
			return S2253.StartRecord(filename, this.device, this.stream) == 0;
		}
		public bool Stop()
		{
			return S2253.StopStream(this.device, this.stream) == 0;
		}
		internal static Stream Open(Device device, int stream)
		{
			Stream result = new Stream(device, stream);
			int firmwareVersion = result.FirmwareVersion;
			return result;
		}
	}
}
