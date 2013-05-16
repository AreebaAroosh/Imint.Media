// 
//  InputControl.cs
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
using Serialize = Kean.Core.Serialize;
using Uri = Kean.Core.Uri;
using Parallel = Kean.Core.Parallel;
using Platform = Kean.Platform;
using Geometry2D = Kean.Math.Geometry2D;
using Settings = Kean.Platform.Settings;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;

namespace Imint.Media.Module
{
    public class InputControl :
        Input,
        IMedia,
		IInputControl
	{
		IControl backend;
		public InputControl(IInputControl backend) :
            base(backend)
        {
			this.backend = backend;
			this.backend.StartChanged += start => this.StartChanged.Call(start + this.Offset);
			this.backend.PositionChanged += position => this.PositionChanged.Call(position + this.Offset);
			this.backend.EndChanged += end => this.EndChanged.Call(end + this.Offset);
			this.OffsetChanged += offset =>
			{
				this.StartChanged.Call(this.Start);
				this.PositionChanged.Call(this.Position);
				this.EndChanged.Call(this.End);
			};
		}
		protected override void Send(Frame frame)
		{
			if (this.Crop.NotNull())
				frame.Crop = this.Crop;
			if (this.Scan != Scan.Unknown)
				frame.Scan = this.Scan;
			if (this.Ratio.NotNull())
				frame.Ratio = (float)this.Ratio;
			frame.Time += this.Offset;
			base.Send(frame);
		}
		protected override void Stop()
		{
			this.Eject();
			base.Stop();
		}
        #region IControl Members
		#region Ratio
		Kean.Math.Fraction ratio = null;
		[Platform.Settings.Property("ratio", "Aspect ratio of the corresponding video source.", "Get/set the video aspect ratio using double value [integer.decimals] or integer fraction [nominator/denominator].")]
		[Notify("RatioChanged")]
		public Kean.Math.Fraction Ratio
		{
			get { return this.ratio; }
			set
			{
				if (this.ratio != value)
				{
					this.ratio = value;
					this.RatioChanged.Call(value);
				}
			}
		}
		public event Action<Kean.Math.Fraction> RatioChanged;
		#endregion
		#region Scan
		Media.Scan scan = Media.Scan.Unknown;
		[Platform.Settings.Property("scan", "Scan format of video", "Get/set the scan format of video [Unknown | Interlaced | Progressive].")]
		[Notify("ScanChanged")]
		public Media.Scan Scan
		{
			get { return this.scan; }
			set
			{
				if (this.scan != value)
				{
					this.scan = value;
					this.ScanChanged.Call(value);
				}
			}
		}
		public event Action<Media.Scan> ScanChanged;
		#endregion
		#region Crop
		Geometry2D.Integer.Shell crop;
		[Platform.Settings.Property("crop", "Crop of video frame borders", "Get/set the video frame borders [left, right, top, bottom].")]
		[Notify("CropChanged")]
		public Geometry2D.Integer.Shell Crop
		{
			get { return this.crop; }
			set
			{
				if (this.crop != value)
				{
					this.crop = value;
					this.CropChanged.Call(value);
				}
			}
		}
		public event Action<Geometry2D.Integer.Shell> CropChanged;
		#endregion

		[Settings.Property("resource", "Resource currently opened.", "The locator of the currently opened resource.")]
		[Notify("ResourceChanged")]
		public Uri.Locator Resource { get { return this.backend.Resource; } }
		public event Action<Uri.Locator> ResourceChanged
		{
			add { this.backend.ResourceChanged += value; }
			remove { this.backend.ResourceChanged -= value; }
		}

		[Settings.Property("state", "State of media.", "The state [closed | paused | playing] of media.")]
		[Notify("StatusChanged")]
		public Status Status { get { return this.backend.Status; } }
        public event Action<Status> StatusChanged
		{
			add { this.backend.StatusChanged += value; }
			remove { this.backend.StatusChanged -= value; }
		}

		#region Offset
		TimeSpan offset;
		[Settings.Property("offset", "Offset position of media.", "The offset position of the media in format [[h:]mm:]ss[.fff].")]
		[Notify("OffsetChanged")]
		public TimeSpan Offset { get { return this.offset; } set { if (this.offset != value) this.OffsetChanged.Call(this.offset = value); } }
		public event Action<TimeSpan> OffsetChanged;
		#endregion

		#region Start
        [Settings.Property("start", "Start position of media.", "The start position of the media in format [[h:]mm:]ss[.fff].")]
        [Notify("StartChanged")]
        public DateTime Start 
		{ 
			get { return this.backend.Start + this.Offset; }
			set { this.Offset = value - this.backend.Start; }
		}
        public event Action<DateTime> StartChanged;
        #endregion

        #region Position
        [Settings.Property("position", "Position of media.", "The current position of the media in format [[h:]mm:]ss[.fff].")]
        [Notify("PositionChanged")]
        public DateTime Position { get { return this.backend.Position + this.Offset; } }
		public event Action<DateTime> PositionChanged;
        #endregion

        #region End
        [Settings.Property("end", "End position of media.", "The end position of the media in format [[h:]mm:]ss[.fff].")]
        [Notify("EndChanged")]
		public DateTime End { get { return this.backend.End + this.Offset; } }
        public event Action<DateTime> EndChanged;
        #endregion

        [Settings.Property("extensions", "Media file extensions.", "Get all media file extensions that can be opened.")]
        public string AllExtensions 
        { 
            get
            {
                string result = "";
                foreach(string extension in this.Extensions)
                   result += extension + " ";
                return result;
            }
        }
		public string[] Extensions { get { return this.backend.Extensions; } }

		[Settings.Method("open", "Opens media.", "Opens media specified by locator argument.")]
		public bool Open(Uri.Locator resource) 
		{
			string crop = resource.Query["crop"];
			if (crop.NotEmpty())
				this.Crop = (Geometry2D.Integer.Shell)crop;
			string scan = resource.Query["scan"];
			if (scan.NotEmpty())
				this.Scan = (Media.Scan)Enum.Parse(typeof(Media.Scan), scan, true);
			this.Ratio = resource.Query["ratio"];
			resource.Query.Remove("crop", "ratio", "scan");
			return this.backend.Open(resource);
		}

		[Settings.Method("play", "Start playback.", "Start playback of opened media.")]
		public void Play() { this.backend.Play(); }

		[Settings.Method("pause", "Pause playback.", "Pause playback when playing.")]
		public void Pause() { this.backend.Pause(); }

		[Settings.Method("eject", "Eject opened media.", "Eject currently opened media.")]
		public void Eject() { this.backend.Eject(); }

        #region Seek
        [Settings.Property("seekable", "Media is seekable bool flag.", "Get bool flag which says if the input media is seekable.")]
        [Notify("SeekableChanged")]
		public bool Seekable { get { return this.backend.Seekable; } }
        public event Action<bool> SeekableChanged
		{
			add { this.backend.SeekableChanged += value; }
			remove { this.backend.SeekableChanged -= value; }
		}
		[Settings.Method("seek", "Seek position of media.", "Seek to current position of the media in format [[h:]mm:]ss[.fff].")]
		public void Seek(DateTime position) { this.backend.Seek(position); }
        #endregion

        #region Next
        [Settings.Property("hasnext", "Media has next bool flag.", "Get bool flag which says if the input media position can seeked to the last captured position.")]
        [Notify("HasNextChanged")]
        public bool HasNext { get { return this.backend.HasNext; } }
        public event Action<bool> HasNextChanged
		{
			add { this.backend.HasNextChanged += value; }
			remove { this.backend.HasNextChanged -= value; }
		}
		[Settings.Method("next", "Go to last captured position.", "The media plays from the last captured position.")]
		public void Next() { this.backend.Next(); }
        #endregion
        
        #region Previous
        [Settings.Property("hasprevious", "Media has previous bool flag.", "Get bool flag which says if the input media position can seeked to the first captured position.")]
        [Notify("HasPreviousChanged")]
        public bool HasPrevious { get { return this.backend.HasPrevious; } }
        public event Action<bool> HasPreviousChanged
		{
			add { this.backend.HasPreviousChanged += value; }
			remove { this.backend.HasPreviousChanged -= value; }
		}
		[Settings.Method("previous", "Go to first captured position.", "The media plays from the first captured position.")]
		public void Previous() { this.backend.Previous(); }
		#endregion
		#endregion
	}
}
