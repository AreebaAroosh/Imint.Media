// 
//  Live.cs
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
using Kean.Extension;
using Bitmap = Kean.Draw.Raster;

namespace Imint.Media.DirectShow.Elecard.Timeshift.Graph
{
	class Live :
		NonLive
	{
		enum Mode
		{
			Live,
			Timeshift,
			Freeze
		}
		const float timeshiftPart = 0.95f;
		object @lock = new object();
		Mode state;
		Mode State
		{
			get { lock (this.@lock) return this.state; }
			set { lock (this.@lock) { this.state = value; } }
		}
		DateTime freezePosition = new DateTime();
		public override bool Play()
		{
			switch (this.State)
			{
				case Mode.Timeshift:
					base.Play();
					break;
				case Mode.Freeze:
					this.Seek(this.freezePosition);
					break;
			}
			return true;
		}
		public override bool Pause()
		{
			switch (this.State)
			{
				case Mode.Timeshift:
					base.Pause();
					break;
				case Mode.Live:
					{
						this.State = Mode.Freeze;
						this.freezePosition = this.Recorder.Position;
					}
					break;
			}
			return true;
		}

		public override void Seek(DateTime position)
		{
			if (position < new DateTime((long)(this.End.Ticks * Live.timeshiftPart)))
			{
				this.State = Mode.Timeshift;
				base.Seek(position);
				base.Play();
			}
			else
			{
				this.State = Mode.Live;
				base.Pause();
			}
		}
		public override DateTime Position
		{
			get
			{

				DateTime result;
				switch (this.State)
				{
					case Mode.Timeshift:
						result = base.LastSeek.AddTicks(base.Position.Ticks);
						break;
					default:
					case Mode.Live:
						result = this.End;
						break;
					case Mode.Freeze:
						result = this.freezePosition;
						break;
				}
				return result;
			}
		}
		public override DateTime Start
		{
			get { return this.Recorder.Start; }
		}
		public override DateTime End
		{
			get
			{
				DateTime result;
				switch (this.State)
				{

					default:
					case Mode.Timeshift:
						result = new DateTime((long)(base.End.Ticks * (1.0f / Live.timeshiftPart)));
						break;
					case Mode.Freeze:
					case Mode.Live:
						result = this.Recorder.Position;
						break;
				}
				return result;
			}
		}
		public override Status Status
		{
			get
			{
				Status result;
				switch (this.State)
				{
					case Mode.Timeshift:
						result = base.Status;
						break;
					default:
					case Mode.Live:
						result = Status.Playing;
						break;
					case Mode.Freeze:
						result = Status.Paused;
						break;
				}
				return result;
			}
		}
		public override Action<DateTime, TimeSpan, Bitmap.Image> Send
		{
			set
			{
				base.Send = (position, valid, data) =>
				{
					if (this.State == Mode.Timeshift)
						value.Call(position, valid, data);
				};
				this.Recorder.Send = (position, valid, data) =>
				{
					if (this.State == Mode.Live)
						value.Call(position, valid, data);
				};
			}
		}
	}
}
