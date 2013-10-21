// 
//  Latest.cs
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
using Kean;
using Kean.Extension;
using Parallel = Kean.Parallel;

namespace Imint.Media.Regulator
{
	public class Latest :
		Synchronized,
		IInput
	{
		public IInput Backend { get; set; }
		public int MaximumActive { get; set; }
		Frame last;
		int activeCount;
		int discardedCount;

		public Latest(IInput backend) :
			this(backend, 1)
		{ }
		public Latest(IInput backend, int maximumActive)
		{
			this.Backend = backend;
			this.MaximumActive = maximumActive;
		}
		public Latest()
		{
			this.MaximumActive = 1;
		}

		#region IInput Members
		public Action<Frame> Send { set; private get; }
		public event Action OnReset;
		public void Initialize(Parallel.ThreadPool threadPool)
		{
			this.Backend.Initialize(threadPool);
			this.Backend.Send = this.Receive;
			this.Backend.OnReset += () =>
			{
				lock (this.Lock)
				{
					this.activeCount = 0;
					this.discardedCount = 0;
				}
				this.OnReset.Call();
			};
		}
		#endregion
		void Receive(Frame frame)
		{
			if (frame.NotNull())
				lock (this.Lock)
				{
					if (this.activeCount >= this.MaximumActive)
					{
						if (this.discardedCount > 10)
							this.activeCount = 0;
						else if (this.last.NotNull())
							this.discardedCount++;
						this.last = this.Drop(this.last, frame);
						this.last.ReferenceCount.Increase();
					}
					else
					{
						this.activeCount++;
						this.discardedCount = 0;
						this.Send.Call(this.Rebuild(frame));
					}
				}
		}
		Frame Drop(Frame old, Frame @new)
		{
			if (old.NotNull())
			{
				old.ReferenceCount.Decrease();
				if (old.Done.NotNull())
				{
					Frame newer = old.Done(true);
					if (newer.NotNull())
						@new = this.Drop(@new, newer);
				}
			}
			return @new;
		}
		Frame Rebuild(Frame frame)
		{
			Func<bool, Frame> old = frame.Done;
			frame.Done = dropped =>
			{
				if (old.NotNull())
					this.Receive(old(dropped));
				Frame result = null;
				lock (this.Lock)
				{
					this.activeCount--;
					if (this.activeCount < this.MaximumActive && this.last.NotNull())
					{
						result = this.Rebuild(this.last);
						this.activeCount++;
						this.discardedCount = 0;
						this.last = null;
					}
				}
				return result;
			};
			return frame;
		}

		#region IDisposable Members
		public void Dispose()
		{
			if (this.Backend.NotNull())
			{
				this.Backend.Dispose();
				this.Backend = null;
			}
			if (this.last.NotNull())
			{
				if (last.ReferenceCount.NotNull())
					this.last.ReferenceCount.Decrease();
				this.last = null;
			}
		}
		#endregion
	}
}
