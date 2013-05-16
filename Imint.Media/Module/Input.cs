// 
//  Input.cs
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
using Uri = Kean.Core.Uri;
using Parallel = Kean.Core.Parallel;
using Platform = Kean.Platform;
using Geometry2D = Kean.Math.Geometry2D;
using Settings = Kean.Platform.Settings;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Serialize = Kean.Core.Serialize;

namespace Imint.Media.Module
{
    public class Input :
        Platform.Module,
        IInput
    {
		IInput backend;
		Action<Frame> send;
		public Input(IInput backend) :
            base("Media")
        {
			this.backend = backend;
			this.backend.Send = this.Send;
			this.backend.OnReset += () => this.OnReset.Call();
		}
        protected override void Initialize()
        {
            this.Application["Settings"].WhenLoaded<Settings.Module>(m => m.Load("media", "Media controls.", "Object that contains all media controls.", this));
			this.Application["ThreadPool"].WhenLoaded<Platform.Module<Parallel.ThreadPool>>(m => (this as IInput).Initialize(m.Value));
			base.Initialize();
        }
		protected override void Stop()
		{
			base.Stop();
		}
		protected virtual void Send(Frame frame)
		{
			this.send(frame);
		}
		#region IInput Members
		Action<Frame> IInput.Send { set { this.send = value; } }
		public event Action OnReset;
		void IInput.Initialize(Parallel.ThreadPool threadPool) 
		{
			this.backend.Initialize(threadPool);
		}
		#endregion
		protected override void Dispose()
		{
			if (this.backend.NotNull())
			{
				this.backend.Dispose();
				this.backend = null;
			}
			base.Dispose();
		}
	}
}
