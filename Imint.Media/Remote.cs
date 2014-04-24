// 
//  Remote.cs
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
using Uri = Kean.Uri;
using Kean.Extension;
using Geometry2D = Kean.Math.Geometry2D;
using Settings = Kean.Platform.Settings;

namespace Imint.Media
{
	public class Remote :
		IMedia
	{
		Settings.Remote backend;

		Remote(Settings.Remote backend)
		{
			this.backend = backend;
		}

		public static Remote Create(Settings.Remote backend)
		{
			return backend.Exists("media") ? new Remote(backend) : null;
		}

		#region Ratio

		public Kean.Math.Fraction Ratio
		{
			get { return this.backend.Get<Kean.Math.Fraction>("media.ratio"); }
			set { this.backend.Set("media.ratio", value); }
		}

		Action<Kean.Math.Fraction> ratioChanged;

		public event Action<Kean.Math.Fraction> RatioChanged
		{
			add
			{
				if (this.ratioChanged.IsNull())
					this.backend.Listen("media.ratio", (Kean.Math.Fraction v) => this.ratioChanged.Call(v));
				this.ratioChanged += value;
			}
			remove { this.ratioChanged -= value; }
		}

		#endregion

		#region Scan

		public Imint.Media.Scan Scan
		{
			get { return this.backend.Get<Imint.Media.Scan>("media.scan"); }
			set { this.backend.Set("media.scan", value); }
		}

		Action<Imint.Media.Scan> scanChanged;

		public event Action<Imint.Media.Scan> ScanChanged
		{
			add
			{
				if (this.scanChanged.IsNull())
					this.backend.Listen("media.scan", (Imint.Media.Scan v) => this.scanChanged.Call(v));
				this.scanChanged += value;
			}
			remove { this.scanChanged -= value; }
		}

		#endregion

		#region Crop

		public Geometry2D.Integer.Shell Crop
		{
			get { return this.backend.Get<Geometry2D.Integer.Shell>("media.crop"); }
			set { this.backend.Set("media.crop", value); }
		}

		Action<Geometry2D.Integer.Shell> cropChanged;

		public event Action<Kean.Math.Geometry2D.Integer.Shell> CropChanged
		{
			add
			{
				if (this.cropChanged.IsNull())
					this.backend.Listen("media.crop", (Geometry2D.Integer.Shell v) => this.cropChanged.Call(v));
				this.cropChanged += value;
			}
			remove { this.cropChanged -= value; }
		}

		#endregion

		#region EndMode

		public Imint.Media.EndMode EndMode
		{
			get { return this.backend.Get<Imint.Media.EndMode>("media.endmode"); }
			set { this.backend.Set("media.endmode", value); }
		}

		Action<Imint.Media.EndMode> endModeChanged;

		public event Action<Imint.Media.EndMode> EndModeChanged
		{
			add
			{
				if (this.endModeChanged.IsNull())
					this.backend.Listen("media.endmode", (Imint.Media.EndMode v) => this.endModeChanged.Call(v));
				this.endModeChanged += value;
			}
			remove { this.endModeChanged -= value; }
		}

		#endregion

		#region Resource

		public Uri.Locator Resource { get { return this.backend.Get<Uri.Locator>("media.resource"); } }

		Action<Uri.Locator> resourceChanged;

		public event Action<Uri.Locator> ResourceChanged
		{
			add
			{
				if (this.resourceChanged.IsNull())
					this.backend.Listen("media.resource", (Uri.Locator v) => this.resourceChanged.Call(v));
				this.resourceChanged += value;
			}
			remove { this.resourceChanged -= value; }
		}

		#endregion

		#region Status

		public Imint.Media.Status Status { get { return this.backend.Get<Imint.Media.Status>("media.state"); } }

		Action<Imint.Media.Status> statusChanged;

		public event Action<Imint.Media.Status> StatusChanged
		{
			add
			{
				if (this.statusChanged.IsNull())
					this.backend.Listen("media.state", (Imint.Media.Status v) => this.statusChanged.Call(v));
				this.statusChanged += value;
			}
			remove { this.statusChanged -= value; }
		}

		#endregion

		#region Start

		public DateTime Start { get { return this.backend.Get<DateTime>("media.start"); } }

		Action<DateTime> startChanged;

		public event Action<DateTime> StartChanged
		{
			add
			{
				if (this.startChanged.IsNull())
					this.backend.Listen("media.start", (DateTime v) => this.startChanged.Call(v));
				this.startChanged += value;
			}
			remove { this.startChanged -= value; }
		}

		#endregion

		#region Position

		public DateTime Position { get { return this.backend.Get<DateTime>("media.position"); } }

		Action<DateTime> positionChanged;

		public event Action<DateTime> PositionChanged
		{
			add
			{
				if (this.positionChanged.IsNull())
					this.backend.Listen("media.position", (DateTime v) => this.positionChanged.Call(v));
				this.positionChanged += value;
			}
			remove { this.positionChanged -= value; }
		}

		#endregion

		#region End

		public DateTime End { get { return this.backend.Get<DateTime>("media.end"); } }

		event Action<DateTime> endChanged;

		public event Action<DateTime> EndChanged
		{
			add
			{
				if (this.endChanged.IsNull())
					this.backend.Listen("media.end", (DateTime v) => this.endChanged.Call(v));
				this.endChanged += value;
			}
			remove { this.positionChanged -= value; }
		}

		#endregion

		#region Seekable

		public bool Seekable { get { return this.backend.Get<bool>("media.seekable"); } }

		Action<bool> seekableChanged;

		public event Action<bool> SeekableChanged
		{
			add
			{
				if (this.seekableChanged.IsNull())
					this.backend.Listen("media.seekable", (bool v) => this.seekableChanged.Call(v));
				this.seekableChanged += value;
			}
			remove { this.seekableChanged -= value; }
		}

		#endregion

		#region HasNext

		public bool HasNext { get { return this.backend.Get<bool>("media.hasnext"); } }

		Action<bool> hasNextChanged;

		public event Action<bool> HasNextChanged
		{
			add
			{
				if (this.hasNextChanged.IsNull())
					this.backend.Listen("media.hasnext", (bool v) => this.hasNextChanged.Call(v));
				this.hasNextChanged += value;
			}
			remove { this.hasNextChanged -= value; }
		}

		#endregion

		#region HasPrevious

		public bool HasPrevious { get { return this.backend.Get<bool>("media.hasprevious"); } }

		Action<bool> hasPreviousChanged;

		public event Action<bool> HasPreviousChanged
		{
			add
			{
				if (this.hasPreviousChanged.IsNull())
					this.backend.Listen("media.hasprevious", (bool v) => this.hasPreviousChanged.Call(v));
				this.hasPreviousChanged += value;
			}
			remove { this.hasPreviousChanged -= value; }
		}

		#endregion

		public System.Collections.Generic.IEnumerable<Resource> Devices
		{ 
			get { return this.backend.Get<string>("media.devices").FromCsv().Map(device => (Resource)device); } 
		}

		public string[] Extensions { get { return this.backend.Get<string>("media.extensions").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries); } }

		public bool Open(Uri.Locator resource)
		{
			return this.backend.Call("media.open", resource);
		}

		public void Play()
		{
			this.backend.Call("media.play");
		}

		public void Pause()
		{
			this.backend.Call("media.pause");
		}

		public void Eject()
		{
			this.backend.Call("media.eject");
		}

		public void Seek(DateTime position)
		{
			this.backend.Call("media.seek", position);
		}

		public void Next()
		{
			this.backend.Call("media.next");
		}

		public void Previous()
		{
			this.backend.Call("media.previous");
		}

		public void Dispose()
		{
		}
	}
}
