// 
//  Singlepart.cs
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
using Collection = Kean.Collection;

namespace Imint.Media.Mjpeg.Http
{
	public abstract class Singlepart :
		   Abstract
	{
		protected Singlepart(Uri.Locator locator, int readSize, int attempts) : 
			base(locator, readSize, attempts) 
		{ }
		protected override void StreamParser(System.Net.WebResponse response, byte[] buffer, int readSize)
		{
			System.IO.Stream stream = response.GetResponseStream();
			int total = 0;
			// loop
			// safe check. Flush the buffer if do not have enough space left.
			while (total + readSize < buffer.Length)
			{
				int read = 0;
				// read == 0 means end of stream.
				if ((read = stream.Read(buffer, total, readSize)) == 0)
					break;
				total += read;
			}
			if (total != 0 && total < buffer.Length)
				this.Send(new System.IO.MemoryStream(buffer, 0, total));
		}
	}
}
