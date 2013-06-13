// 
//  Unbuffered.cs
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
using Kean.Core.Collection.Extension;
using Serialize = Kean.Core.Serialize;
using Uri = Kean.Core.Uri;
using Raster = Kean.Draw.Raster;
using Parallel = Kean.Core.Parallel;
using Collection = Kean.Core.Collection;
using Geometry2D = Kean.Math.Geometry2D;

namespace Imint.Media.Input
{
	public class Unbuffered :
		Synchronized,
		IInputControl
	{
		long dropCount;
		bool dropNext;
		Players players;
		[Serialize.Parameter("Player")]
		public Players Players { 
			get { return this.players; }
			internal set 
			{ 
				this.players = value;
				if (this.players.NotNull())
				{
					this.players.UpdateState = this.UpdateState;
					this.players.Send = this.SendFrame;
				}
			}
		}
		[Serialize.Parameter]
		public EndMode EndMode { get; set; }

		Action<Frame> send;

		protected Parallel.ThreadPool ThreadPool { get; private set; }

		public Unbuffered()
		{
			this.Players = new Players();
		}
		#region IDisposable Members
		public void Dispose()
		{
			if (this.Players.NotNull())
			{
				this.Players.Close();
				this.Players = null;
			}
		}
		#endregion

		protected virtual void UpdateState(bool closed, bool playing, DateTime start, DateTime end, DateTime position, bool isLinear, bool isNonLinear, bool hasNext)
		{
			this.Status = closed ? Status.Closed : playing ? Status.Playing : Status.Paused;
			this.Position = position;
			this.Start = start;
			this.End = end;
			this.Length = end - start;
			this.Seekable = isNonLinear;
			this.HasEnd = isNonLinear;
			this.HasStart = isNonLinear;
			this.HasPosition = isLinear;
			this.HasPrevious = isNonLinear;
			this.HasNext = hasNext;
			if (position.AddMilliseconds(120) > end && playing && this.Seekable && !this.HasNext)
			{
				switch (this.EndMode)
				{
					case EndMode.Pause:
						this.Pause();
						break;
					default:
					case EndMode.Play:
						break;
					case EndMode.Repeat:
						this.Seek(start);
						break;
					case EndMode.Eject:
						this.Pause();
						System.Threading.Thread.Sleep(80);
						this.Eject();
						break;
				}
			}
		}
		protected virtual void SendFrame(Frame frame)
		{
			bool drop;
			lock (this.Lock)
				if (drop = this.dropNext)
					this.dropNext = false;
			if (!drop)
				this.send(frame);
			else
				this.dropCount++;

		}
		protected virtual void Done(bool droped)
		{
			if (droped)
				this.dropCount++;
		}
		public event Func<Uri.Locator, Uri.Locator> OpenFilter;
		public virtual bool Open(Uri.Locator resource)
		{
			this.Resource = resource;
			if (this.OpenFilter.NotNull())
				resource = this.OpenFilter(resource);
			lock (this.Lock)
				return this.Players.Open(resource);
		}
		public virtual void Play()
		{
			lock (this.Lock)
			{
				if (this.EndMode == EndMode.Pause && this.Position.AddMilliseconds(120) > this.End && this.Seekable)
					this.Seek(this.Start);
				this.Players.Play();
			}
		}
		public virtual void Pause()
		{
			lock (this.Lock)
				this.Players.Pause();
		}
		public virtual void Eject()
		{
			lock (this.Lock)
			{
				this.Players.Close();
				this.Status = Media.Status.Closed;
				this.Position = new DateTime();
				this.Start = new DateTime();
				this.End = new DateTime();
				this.Length = new TimeSpan();
				this.HasStart = false;
				this.HasPosition = false;
				this.HasEnd = false;
				this.Seekable = false;
				this.HasPrevious = false;
				this.HasNext = false;
				this.Resource = null;
			}
		}
		public virtual void Seek(DateTime position)
		{
			lock (this.Lock)
				this.Players.Seek(position);
		}
		#region Next
		bool hasNext;
		public virtual bool HasNext
		{
			get { lock (this.Lock) return this.hasNext; }
			protected set
			{
				lock (this.Lock)
					if (this.HasNext != value)
					{
						this.hasNext = value;
						this.ThreadPool.Enqueue(this.HasNextChanged.Call, value);
					}
			}
		}
		public virtual event Action<bool> HasNextChanged;
		public virtual void Next()
		{ }
		#endregion
		#region Previous
		bool hasPrevious;
		public virtual bool HasPrevious
		{
			get { lock (this.Lock) return this.hasPrevious; }
			protected set
			{
				lock (this.Lock)
					if (this.HasPrevious != value)
					{
						this.hasPrevious = value;
						this.ThreadPool.Enqueue(this.HasPreviousChanged.Call, value);
					}
			}
		}
		public virtual void Previous()
		{ }
		#endregion
		protected virtual void Reset()
		{ }

		#region IControl Members
		Uri.Locator resource;
		public virtual Uri.Locator Resource 
		{
			get { return this.resource; }
			protected set 
			{
				if (!this.resource.SameOrEquals(value))
				{
					this.resource = value;
					this.ResourceChanged.Call(value);
				}
			}
		}
		public virtual event Action<Uri.Locator> ResourceChanged;
		Status status;
		public virtual Status Status
		{
			get { lock (this.Lock) return this.status; }
			protected set
			{
				lock (this.Lock)
					if (this.status != value)
					{
						if (value == Status.Closed)
							this.OnReset.Call();
						this.status = value;
						this.ThreadPool.Enqueue(this.StatusChanged.Call, value);
					}
			}
		}
		public virtual event Action<Status> StatusChanged;
		#region Start
		bool hasStart;
		public virtual bool HasStart
		{
			get { lock (this.Lock) return this.hasStart; }
			protected set
			{
				lock (this.Lock)
					if (this.hasStart != value)
					{
						this.hasStart = value;
						this.ThreadPool.Enqueue(this.HasStartChanged.Call, value);
					}
			}
		}
		public virtual event Action<bool> HasStartChanged;
		DateTime start;
		public virtual DateTime Start
		{
			get { lock (this.Lock) return this.start; }
			protected set
			{
				lock (this.Lock)
					if (this.Start != value)
					{
						this.start = value;
						this.ThreadPool.Enqueue(this.StartChanged.Call, value);
					}
			}
		}
		public virtual event Action<DateTime> StartChanged;
		#endregion
		#region End
		bool hasEnd;
		public virtual bool HasEnd
		{
			get { lock (this.Lock) return this.hasEnd; }
			protected set
			{
				lock (this.Lock)
					if (this.hasEnd != value)
					{
						this.hasEnd = value;
						this.ThreadPool.Enqueue(this.HasEndChanged.Call, value);
					}
			}
		}
		public virtual event Action<bool> HasEndChanged;
		DateTime end;
		public virtual DateTime End
		{
			get { lock (this.Lock) return this.end; }
			protected set
			{
				lock (this.Lock)
					if (this.End != value)
					{
						this.end = value;
						this.ThreadPool.Enqueue(this.EndChanged.Call, value);
					}
			}
		}
		public virtual event Action<DateTime> EndChanged;
		#endregion
		#region Length
		TimeSpan length;
		public virtual TimeSpan Length
		{
			get { lock (this.Lock) return this.length; }
			protected set
			{
				lock (this.Lock)
					if (this.Length != value)
					{
						this.length = value;
						this.ThreadPool.Enqueue(this.LengthChanged.Call, value);
					}
			}
		}
		public virtual event Action<TimeSpan> LengthChanged;
		#endregion
		#region Position
		bool hasPosition;
		public virtual bool HasPosition
		{
			get { lock (this.Lock) return this.hasPosition; }
			protected set
			{
				lock (this.Lock)
					if (this.hasPosition != value)
					{
						this.hasPosition = value;
						this.ThreadPool.Enqueue(this.HasPositionChanged.Call, value);
					}
			}
		}
		public virtual event Action<bool> HasPositionChanged;
		DateTime position;
		public virtual DateTime Position
		{
			get { lock (this.Lock) return this.position; }
			protected set
			{
				lock (this.Lock)
					if (this.Position != value)
					{
						this.position = value;
						this.ThreadPool.Enqueue(this.PositionChanged.Call, value);
					}
			}
		}
		public virtual event Action<DateTime> PositionChanged;
		#endregion
		
		bool seekable;
		public virtual bool Seekable
		{
			get { lock (this.Lock) return this.seekable; }
			protected set
			{
				lock (this.Lock)
					if (this.Seekable != value)
					{
						this.seekable = value;
						this.ThreadPool.Enqueue(this.SeekableChanged.Call, value);
					}
			}
		}
		public virtual event Action<bool> SeekableChanged;

		public virtual event Action<bool> HasPreviousChanged;

		public System.Collections.Generic.IEnumerable<Resource> Devices 
		{ 
			get 
			{
				foreach (Player.IStream player in this.Players)
					if (player is Player.ICapture)
						foreach (Resource device in (player as Player.ICapture).Devices)
							yield return device;
			} 
		}
		string[] extensions;
		public virtual string[] Extensions
		{
			get { lock (this.Lock) return this.extensions; }
			protected set { lock (this.Lock) this.extensions = value; }
		}
		#endregion
		#region IInput Members
		public virtual Action<Frame> Send { set { this.send = value; } }
		public event Action OnReset;
		public virtual void Initialize(Parallel.ThreadPool threadPool)
		{
			this.ThreadPool = threadPool;
			this.Extensions = this.Players.SupportedExtensions.ToArray();
		}
		#endregion
	}
}
