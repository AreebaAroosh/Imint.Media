// 
//  InputControlWrapper.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2014 Imint AB
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
using Generic = System.Collections.Generic;
using Kean;
using Kean.Extension;
using Serialize = Kean.Serialize;
using Uri = Kean.Uri;
using Parallel = Kean.Parallel;
using Platform = Kean.Platform;
using Geometry2D = Kean.Math.Geometry2D;
using Settings = Kean.Platform.Settings;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Collection;

namespace Imint.Media
{
	public abstract class InputControlWrapper :
		IInputControl
	{
		public int Priority { get; set; }
		IInputControl backend;
		internal protected IInputControl Backend 
		{ 
			get { return this.backend; } 
			internal set 
			{
				if (this.backend != value)
				{
					if (this.backend.NotNull())
					{
						this.backend.EndChanged -= this.OnEndChanged;
						this.backend.EndModeChanged -= this.OnEndModeChanged;
						this.backend.HasNextChanged -= this.OnHasNextChanged;
						this.backend.HasPreviousChanged -= this.OnHasPreviousChanged;
						this.backend.PositionChanged -= this.OnPositionChanged;
						this.backend.ResourceChanged -= this.OnResourceChanged;
						this.backend.SeekableChanged -= this.OnSeekableChanged;
						this.backend.StartChanged -= this.OnStartChanged;
						this.backend.StatusChanged -= this.OnStatusChanged;
					}
					this.backend = value;
					if (this.backend.NotNull())
					{
						this.Send = this.OnSend;
						this.backend.EndChanged += this.OnEndChanged;
						this.backend.EndModeChanged += this.OnEndModeChanged;
						this.backend.HasNextChanged += this.OnHasNextChanged;
						this.backend.HasPreviousChanged += this.OnHasPreviousChanged;
						this.backend.PositionChanged += this.OnPositionChanged;
						this.backend.ResourceChanged += this.OnResourceChanged;
						this.backend.SeekableChanged += this.OnSeekableChanged;
						this.backend.StartChanged += this.OnStartChanged;
						this.backend.StatusChanged += this.OnStatusChanged;
					}
				}
			}
		}
		protected InputControlWrapper()
		{
		}
		#region IInputControl
		public Action<Frame> Send { set; private get; }
		protected virtual void OnSend(Frame frame)
		{
			this.Send(frame);
		}
		public event Action Resetting;

		public virtual void Initialize(Parallel.ThreadPool threadPool)
		{
			this.Backend.Initialize(threadPool);
		}
		public virtual void Dispose()
		{
			if (this.Backend.NotNull())
			{
				this.Backend.Dispose();
				this.Backend = null;
			}
		}
		#region Resource
		public virtual Uri.Locator Resource { get { return this.Backend.Resource; } }
		public event Action<Uri.Locator> ResourceChanged;
		protected virtual void OnResourceChanged(Uri.Locator resource)
		{
			this.ResourceChanged.Call(resource);
		}
		#endregion
		#region Status
		public virtual Status Status
		{
			get { return this.backend.Status; }
		}

		public event Action<Status> StatusChanged;
		protected virtual void OnStatusChanged(Status status)
		{
			this.StatusChanged.Call(status);
		}
		#endregion
		#region Start
		public virtual DateTime Start
		{
			get { return this.backend.Start; }
		}

		public event Action<DateTime> StartChanged;
		protected virtual void OnStartChanged(DateTime dateTime)
		{
			this.StartChanged.Call(dateTime);
		}
		#endregion
		#region Position
		public virtual DateTime Position
		{
			get { return this.backend.Position; }
		}

		public event Action<DateTime> PositionChanged;
		protected virtual void OnPositionChanged(DateTime dateTime)
		{
			this.PositionChanged.Call(dateTime);
		}
		#endregion
		#region End
		public virtual DateTime End
		{
			get { return this.backend.End; }
		}

		public event Action<DateTime> EndChanged;
		protected virtual void OnEndChanged(DateTime dateTime)
		{
			this.EndChanged.Call(dateTime);
		}
		#endregion
		#region EndMode
		public virtual EndMode EndMode
		{
			get
			{
				return this.backend.EndMode;
			}
			set
			{
				this.backend.EndMode = value;
			}
		}

		public event Action<EndMode> EndModeChanged;
		protected virtual void OnEndModeChanged(EndMode endMode)
		{
			this.EndModeChanged.Call(endMode);
		}
		#endregion
		#region Seekable
		public bool Seekable
		{
			get { return this.backend.Seekable; }
		}

		public event Action<bool> SeekableChanged;
		protected virtual void OnSeekableChanged(bool seekable)
		{
			this.SeekableChanged.Call(seekable);
		}
		#endregion
		#region HasNext
		public virtual bool HasNext
		{
			get { return this.backend.HasNext; }
		}

		public event Action<bool> HasNextChanged;
		protected virtual void OnHasNextChanged(bool hasNext)
		{
			this.HasNextChanged.Call(hasNext);
		}
		#endregion
		#region HasPrevious
		public virtual bool HasPrevious
		{
			get { return this.backend.HasPrevious; }
		}

		public event Action<bool> HasPreviousChanged;
		protected virtual void OnHasPreviousChanged(bool hasPrevious)
		{
			this.HasPreviousChanged.Call(hasPrevious);
		}
		#endregion
		public virtual Generic.IEnumerable<Resource> Devices
		{
			get { return this.backend.Devices; }
		}

		public virtual string[] Extensions
		{
			get { return this.backend.Extensions; }
		}

		public virtual bool Open(Kean.Uri.Locator resource)
		{
			return this.backend.Open(resource);
		}

		public virtual void Play()
		{
			this.backend.Play();
		}

		public virtual void Pause()
		{
			this.backend.Pause();
		}

		public virtual void Eject()
		{
			this.backend.Eject();
		}

		public virtual void Seek(DateTime position)
		{
			this.backend.Seek(position);
		}

		public virtual void Next()
		{
			this.backend.Next();
		}

		public virtual void Previous()
		{
			this.backend.Previous();
		}
		#endregion
	}
}
