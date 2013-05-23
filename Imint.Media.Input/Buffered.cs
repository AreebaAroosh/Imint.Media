// 
//  Buffered.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2010-2013 Imint AB
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Kean.Core;
using Kean.Core.Extension;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Error = Kean.Core.Error;
using Geometry2D = Kean.Math.Geometry2D;
using Parallel = Kean.Core.Parallel;
using Kean.Core.Collection.Extension;
using Uri = Kean.Core.Uri;

namespace Imint.Media.Input
{
    public abstract class Buffered :
        Unbuffered
    {
        object @lock = new object();
        bool playerPlaying;
        bool closed;
        DateTime start { get; set; }
        DateTime end { get; set; }
        DateTime playerPosition { get; set; }

        DateTime position;

        bool autoStarted;
        bool playing;
        protected virtual bool Playing
        {
            get { return this.playing; }
            set
            {
                if (this.playing != value && this.Status != Status.Closed)
                {
                    this.playing = value;
                    this.UpdateState();
                }
            }
        }
        Collection.IQueue<Frame> buffer = new Collection.Synchronized.Queue<Frame>(new Collection.Queue<Frame>());
        public Buffered()
        { }
        public virtual bool Send()
        {
            lock (this.@lock)
            {
                bool result;
                if (result = this.buffer.Count > 0)
                {
                    Frame frame = this.buffer.Dequeue();
                    this.position = frame.Time;
                    base.SendFrame(frame);
                }

				if (this.buffer.Count == 0)
				{
					this.Pause();
					this.autoStarted = false;
				}
				if (!this.playerPlaying && this.buffer.Count < 30)
                    base.Play();
                if (this.playerPlaying && this.buffer.Count > 60)
                    base.Pause();
                this.UpdateState();
				return result;
            }
        }
        protected override void Reset()
        {
            lock (this.@lock)
            {
                base.Reset();
                this.autoStarted = false;
                this.Playing = false;
                base.UpdateState(true, false, this.start, this.end, this.position, true, true, false);
            }
        }
        public override bool Open(Uri.Locator resource)
        {
            lock (this.@lock)
            {
                bool result = base.Open(resource);
                if (result)
                    base.Play();
                return result;
            }
        }
        public override void Play()
        {
            lock (this.@lock)
                this.Playing = true;
        }
        public override void Pause()
        {
            lock (this.@lock)
                this.Playing = false;
        }
        public override void Seek(DateTime position)
        {
            lock (this.@lock)
            {
                this.buffer.Clear();
                base.Seek(position);
            }
        }
        protected virtual void UpdateState()
        {
            base.UpdateState(this.closed, this.Playing, this.start, this.end, this.position, true, true, false);
        }
        protected override void UpdateState(bool closed, bool playing, DateTime start, DateTime end, DateTime position, bool isLinear, bool isNonLinear, bool hasNext)
        {
            lock (this.@lock)
            {
                this.closed = closed;
                this.playerPlaying = playing;
                this.start = start;
                this.end = end;
            }
        }
		protected override void  SendFrame(Frame frame)
        {
            lock (this.@lock)
            {
                this.buffer.Enqueue(frame);
                if (!this.autoStarted && this.buffer.Count > 40)
                {
                    this.autoStarted = true;
                    this.Play();
                }
            }
        }
    }
}