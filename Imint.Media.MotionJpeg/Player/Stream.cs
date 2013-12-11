// 
//  Stream.cs
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
using IO = Kean.IO;
using Parallel = Kean.Parallel;
using Serialize = Kean.Serialize;

namespace Imint.Media.MotionJpeg.Player
{
	public class Stream :
		Media.Player.IStream
	{
		public int Channels { get { return 1; } }
		public Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { set; private get; }
		public Status Status { get; private set; }

		Http.Response response;
		Parallel.RepeatThread thread;
		[Serialize.Parameter]
		public TimeSpan TimeOut { get; set; }
		public Stream()
		{
			this.TimeOut = TimeSpan.FromSeconds(2);
		}
		~Stream()
		{
			Error.Log.Wrap((Action)this.Close)();
		}
		long frameCount;
		public void Poll() { System.Threading.Thread.Sleep(10); }
		public bool Open(Uri.Locator url)
		{
			bool result = false;
			switch (url.Scheme)
			{
				case "http":
				case "https":
					if (this.thread.IsNull() && this.response.IsNull())
					{
						this.response = new Http.Request() { Url = url }.Connect();
						System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
						this.thread = Parallel.RepeatThread.Start("MotionJpegPlayer", () =>
						{
							if (!this.response.Open((contentType, device) =>
								{
									bool r = true;
									switch (contentType)
									{
										case "image/jpeg":
										case "image/png": // TODO: does png really work with Raster.Image.Open?
											if (wait.NotNull())
												wait.Set();
											Raster.Image image = Raster.Image.Open(device);
											if (image.NotNull())
												this.Send(0, DateTime.Now, TimeSpan.FromSeconds(1 / 25.0f), image, null);
											break;
										default:
											r = false;
											break;
									}
									return r;
								}))
								this.thread.Abort();
						});
						if (!(result = wait.WaitOne(this.TimeOut)))
						{
							this.thread.Abort();
							this.thread.Dispose();
							this.thread = null;
							this.response.Close();
							this.response = null;
						}
						wait.Dispose();
						wait = null;
					}
					break;
			}
			this.Status = result ? Status.Playing : Status.Closed;
			return result;
		}

		public void Close()
		{
			if (this.response.NotNull())
			{
				this.response.Close();
				this.response = null;
			}
			if (this.thread.NotNull())
			{
				this.thread.Abort();
				this.thread.Dispose();
				this.thread = null;
			}
		}
		void IDisposable.Dispose()
		{
			this.Close();
		}

	}

}
