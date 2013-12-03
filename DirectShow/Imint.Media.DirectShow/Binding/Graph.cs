// 
//  Graph.cs
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
using Buffer = Kean.Buffer;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Uri;
using Platform = Kean.Platform;
using Error = Kean.Error;
using Parallel = Kean.Parallel;

namespace Imint.Media.DirectShow.Binding
{
	public class Graph :
		IGraph,
		IBuild,
		IDisposable
	{

		DirectShowLib.IFilterGraph2 graph;
		Action playing;
		Parallel.RepeatThread eventPoller;
		Action onClose;
		public Graph()
		{
		}
		public Graph(Platform.Application application)
		{
			this.Application = application;
		}
		~Graph()
		{
			Error.Log.Wrap((Action)this.Dispose)();
		}
		#region IDisposable Members
		public void Dispose()
		{
			(this as IGraph).Close();
		}
		#endregion
		public DateTime LastSeek { get; private set; }
		public DateTime CurrentPosition { get; private set; }
		#region IGraph Members
		public virtual DateTime Position
		{
			get
			{
				long duration = 0;
				if (this.graph is DirectShowLib.IMediaSeeking)
				{
					Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).SetTimeFormat(DirectShowLib.TimeFormat.MediaTime));
					Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).GetCurrentPosition(out duration));
				}
				return new DateTime(duration);
			}
		}
		public virtual DateTime Start {get { return new DateTime(); } }
		public virtual DateTime End
		{
			get
			{
				long duration = 0;
				if (this.graph is DirectShowLib.IMediaSeeking)
				{
					Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).SetTimeFormat(DirectShowLib.TimeFormat.MediaTime));
					Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).GetDuration(out duration));
				}
				return new DateTime(duration);
			}
		}
		public virtual Status Status
		{
			get
			{
				Status result = Status.Closed;
				if (this.graph is DirectShowLib.IMediaControl)
				{
					DirectShowLib.FilterState state;
					Exception.GraphError.Check((this.graph as DirectShowLib.IMediaControl).GetState(40, out state));
					switch (state)
					{
						case DirectShowLib.FilterState.Stopped:
						case DirectShowLib.FilterState.Paused:
							result = Status.Paused;
							break;
						case DirectShowLib.FilterState.Running:
							result = Status.Playing;
							break;
					}
				}
				return result;
			}
		}
		public virtual bool Play()
		{
			bool result;
			if (result = this.graph is DirectShowLib.IMediaControl)
			{
				result &= (this.graph as DirectShowLib.IMediaControl).Run() == 1;
				this.playing.Call();
			}
			return result;
		}
		public virtual bool Pause()
		{
			bool result;
			if (result = this.graph is DirectShowLib.IMediaControl)
				result &= (this.graph as DirectShowLib.IMediaControl).Pause() == 1;
			return result;
		}
		public virtual bool Stop()
		{
			bool result;
			if (result = this.graph is DirectShowLib.IMediaControl)
				result &= (this.graph as DirectShowLib.IMediaControl).Stop() == 1;
			return result;
		}
		public virtual bool Open(Filters.Abstract recipe)
		{
			this.graph = new DirectShowLib.FilterGraph() as DirectShowLib.IFilterGraph2;
			DirectShowLib.IMediaEventEx mediaEvent = this.graph as DirectShowLib.IMediaEventEx;
			if (mediaEvent.NotNull())
			{
				if (this.eventPoller.NotNull())
					this.eventPoller.Dispose();
				this.eventPoller = Parallel.RepeatThread.Start("DS Event Poller", () =>
				{
					DirectShowLib.EventCode code;
					IntPtr parameter1, parameter2;
					if (mediaEvent.GetEvent(out code, out parameter1, out parameter2, 960) == 0)
					{
						switch (code)
						{
							default:
								Error.Log.Append(Error.Level.Debug, "DirectShow Event: " + code, "DirectShow event " + code + " occured. (paramenter1: " + parameter1 + ", paramenter2: " + parameter2 + ")");
								break;
							case DirectShowLib.EventCode.GraphChanged:
								Error.Log.Append(Error.Level.Debug, "DirectShow Event: Graph Changed", "The DirectShow graph has changed.");
								break;
							case DirectShowLib.EventCode.Paused:
								Error.Log.Append(Error.Level.Debug, "DirectShow Event: Paused", "The DirectShow graph has completed a pause request.");
								break;
							case DirectShowLib.EventCode.ClockChanged:
								Error.Log.Append(Error.Level.Debug, "DirectShow Event: Clock Changed", "The DirectShow graph has changed the reference clock.");
								break;
							case DirectShowLib.EventCode.StErrStPlaying:
								Error.Log.Append(Error.Level.Debug, "DirectShow Event: Stream Error, Still Playing", "A stream error occured, trying to recover by issuing stop followed by play. (paramenter: " + parameter1 + ")");
								this.Stop();
								this.Play();
								break;
						}
					}
				});
			}
			return recipe.Build(this);
		}
		public virtual void Close()
		{
			if (this.graph is DirectShowLib.IMediaControl)
			{
				this.Stop();
				this.onClose.Call();
				this.onClose = null;
				this.Send = null;
				if (this.eventPoller.NotNull())
				{
					this.eventPoller.Abort();
					this.eventPoller = null;
				}
				Exception.GraphError.Check((this.graph as DirectShowLib.IMediaControl).StopWhenReady());
				Exception.GraphError.Check(this.graph.Abort());
				//System.Runtime.InteropServices.Marshal.ReleaseComObject(this.graph);
				this.graph = null;
			}
		}
		public virtual void Seek(DateTime position)
		{
			if (this.graph is DirectShowLib.IMediaSeeking)
			{
				long current = 0;
				long end = 0;
				Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).SetTimeFormat(DirectShowLib.TimeFormat.MediaTime));
				Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).GetPositions(out current, out end));
				Exception.GraphError.Check((this.graph as DirectShowLib.IMediaSeeking).SetPositions(position.Ticks,
																						 DirectShowLib.AMSeekingSeekingFlags.AbsolutePositioning,
																						 end,
																						 DirectShowLib.AMSeekingSeekingFlags.NoPositioning));
				this.LastSeek = position;
			}
		}
		public virtual Action<DateTime, TimeSpan, Bitmap.Image> Send { set; get; }
		public bool Save(Kean.Uri.Locator locator)
		{
			bool result = false;
			if (result = this.graph.NotNull())
				DirectShowLib.Utils.FilterGraphTools.SaveGraphFile(this.graph, locator.PlatformPath);
			return result;
		}
		#endregion

		#region IBuild Members
		DirectShowLib.IFilterGraph2 Binding.IBuild.Graph { get { return this.graph; } }
		event Action IBuild.Playing 
		{
			add { this.playing += value; }
			remove { this.playing -= value; }
		}
		event Action IBuild.OnClose
		{
			add { this.onClose += value; }
			remove { this.onClose -= value; }
		}
		Action<TimeSpan, TimeSpan, Bitmap.Image> Binding.IBuild.Send
		{
			get
			{
				return (TimeSpan position, TimeSpan lifetime, Bitmap.Image bitmap) =>
				{
					this.CurrentPosition = new DateTime(position.Ticks);
					this.Send.Call(new DateTime(position.Ticks), lifetime, bitmap);
				};
			}
		}
		public Platform.Application Application { get; private set; }
		#endregion
		public static System.Collections.Generic.IEnumerable<string> Devices
		{
			get
			{
				foreach (DirectShowLib.DsDevice device in DirectShowLib.DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice))
					yield return device.Name;
			}
		}

	}
}
