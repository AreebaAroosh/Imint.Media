// 
//  Abstract.cs
//  
//  Author:
//       Anders Frisk <andersfrisk77@gmail.com>
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2011 Anders Frisk
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
using Uri = Kean.Uri;
using Raster = Kean.Draw.Raster;
using Parallel = Kean.Parallel;
using Error = Kean.Error;
using Collection = Kean.Collection;

namespace Imint.Media.Mjpeg.Http
{
	public abstract class Abstract
	{
		protected abstract string Type { get; }
		int attempts;
		Uri.Locator url;
		Parallel.Thread thread;
		int readSize;
		bool stopped;
		protected Abstract()
		{ }
		protected Abstract(Uri.Locator url, int readSize, int attempts)
		{
			this.url = url;
			this.readSize = readSize;
			this.attempts = attempts;
		}
		public event Action<Raster.Image> OnFrame;
		public bool Running { get { return !this.stopped; } }
		public bool Start()
		{
			this.Initialize();
			this.stopped = false;
			this.thread = Parallel.Thread.Start("Mjpeg.Http", () =>
			{
				for (int i = 0; i < this.attempts; i++)
					Error.Log.Call(() => 
					{
						byte[] buffer = new byte[512 * 1024];
						while (this.Running)
						{
							System.Net.HttpWebRequest request = System.Net.WebRequest.Create(this.url.ToString()) as System.Net.HttpWebRequest;
							request.Credentials = this.url.Authority.User.NotNull() && this.url.Authority.User.Name.NotEmpty() && this.url.Authority.User.Password.NotEmpty() ? new System.Net.NetworkCredential(this.url.Authority.User.Name, this.url.Authority.User.Password) : null;
							// get response
							System.Net.WebResponse response = request.GetResponse();
							string content = response.ContentType;
							if (!content.StartsWith(this.Type))
							{
								this.stopped = true;
								break;
							}
							this.StreamParser(response, buffer, this.readSize);
							response.Close();
							request.Abort();
						}
					});
			});
			return true;
		}
		public bool Stop()
		{
			return this.stopped = true;
		}
		protected abstract void StreamParser(System.Net.WebResponse response, byte[] buffer, int readSize);
		protected virtual void Initialize()
		{ }
		protected void Send(System.IO.Stream stream)
		{
			this.OnFrame.Call(Raster.Image.Open(stream));
		}
	}
}
