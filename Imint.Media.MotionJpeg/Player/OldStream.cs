// 
//  OldStream.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2012-2013 Imint AB
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
using Raster = Kean.Draw.Raster;
using Uri = Kean.Uri;
using Geometry2D = Kean.Math.Geometry2D;
using Buffer = Kean.Buffer;
using Error = Kean.Error;

namespace Imint.Media.MotionJpeg.Player
{
	public class OldStream :
		Media.Player.IStream
	{
		public int Channels { get { return 1; } }
		public Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { set; private get; }
		public Status Status { get; private set; }

		Http.Abstract decoder;

		public OldStream()
		{ }
		~OldStream()
		{
			Error.Log.Wrap((Action)this.Close)();
		}
		public void Poll() { System.Threading.Thread.Sleep(10); }
		public bool Open(Uri.Locator url)
		{
			bool result = false;
			switch (url.Scheme)
			{
				case "http":
				case "https":
					this.decoder = new Http.Mjpeg(url);
					this.decoder.OnFrame += image => this.Send(0, DateTime.Now, TimeSpan.FromSeconds(1 / 25.0f), image, null);
					result = this.decoder.Start();
					break;
			}
			this.Status = result ? Status.Playing : Status.Closed;
			return result;
		}

		public void Close()
		{
			if (this.decoder.NotNull())
			{
				this.decoder.Stop();
				this.decoder = null;
			}
		}
		void IDisposable.Dispose()
		{
			this.Close();
		}

	}

}
