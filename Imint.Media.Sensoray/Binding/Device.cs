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
using Kean;
using Kean.Extension;
using Collection = Kean.Collection;
using Kean.Collection.Extension;
using S2253 = Sensoray.S2253;

namespace Imint.Media.Sensoray.Binding
{
	public class Device
	{
		public Board Board { get; private set; }
		internal int device;

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
						result = this.streams[stream] = Stream.Open(this, stream);
				}
				return result;
			}
		}

		public uint Serial
		{
			get
			{
				uint result = 0;
				S2253.GetSerialNumber(ref result, this.device);
				return result;
			}
		}
		public DateTime Clock
		{
			set
			{
				S2253.MID2253CLOCK clock = new S2253.MID2253CLOCK() { sec = Convert.ToUInt32((DateTime.Now - new DateTime(1970, 1, 1)).TotalSeconds), usec = 0 };
				S2253.SetClock(ref clock, this.device);
			}
		}
		public VideoSystem VideoSystem
		{
			get 
			{
				S2253.MID2253_VIDSYS result = S2253.MID2253_VIDSYS.MID2253_VIDSYS_PAL;
				S2253.GetVidSys(ref result, this.device);
				return result == S2253.MID2253_VIDSYS.MID2253_VIDSYS_NTSC ? VideoSystem.Ntsc : VideoSystem.Pal;
			}
			set { S2253.SetVidSys(value == VideoSystem.Ntsc ? S2253.MID2253_VIDSYS.MID2253_VIDSYS_NTSC : S2253.MID2253_VIDSYS.MID2253_VIDSYS_PAL, this.device); }
		}
		public bool Deinterlace
		{
			set { S2253.SetInterpolateMode(value ? 1 : 0, this.device); }
			get 
			{
				int result = 0;
				S2253.GetInterpolateMode(ref result, this.device);
				return result == 1;
			}
		}
		public int Hue
		{
			get 
			{
				int result = 0;
				S2253.GetLevel(S2253.MID2253_LEVEL_HUE, ref result, this.device);
				return result;
			}
			set { S2253.SetLevel(S2253.MID2253_LEVEL_HUE, value, this.device); }
		}
		public int Saturation
		{
			get
			{
				int result = 128;
				S2253.GetLevel(S2253.MID2253_LEVEL_SATURATION, ref result, this.device);
				return result;
			}
			set { S2253.SetLevel(S2253.MID2253_LEVEL_SATURATION, value, this.device); }
		}
		public int Contrast
		{
			get
			{
				int result = 128;
				S2253.GetLevel(S2253.MID2253_LEVEL_CONTRAST, ref result, this.device);
				return result;
			}
			set { S2253.SetLevel(S2253.MID2253_LEVEL_CONTRAST, value, this.device); }
		}
		public int Brightness
		{
			get
			{
				int result = 128;
				S2253.GetLevel(S2253.MID2253_LEVEL_BRIGHTNESS, ref result, this.device);
				return result;
			}
			set { S2253.SetLevel(S2253.MID2253_LEVEL_BRIGHTNESS, value, this.device); }
		}

		Device(Board board, int identifier)
		{
			this.Board = board;
			this.device = identifier;
		}

		internal static Device Open(Board board, int device)
		{
			Device result = new Device(board, device);
			uint serial = result.Serial;
			result.Clock = DateTime.Now;
			result.VideoSystem = VideoSystem.Pal;
			return result;
		}
	}
}
