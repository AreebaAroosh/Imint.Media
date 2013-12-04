// 
//  Multipart.cs
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
using Collection = Kean.Collection;

namespace Imint.Media.Mjpeg.Http
{
	public abstract class Multipart :
		Abstract
	{
		protected override string Type { get { return "multipart/x-mixed-replace"; } }
		protected abstract string Subtype { get; }
		public Multipart(Uri.Locator locator, int readSize, int attempts) : 
			base(locator, readSize, attempts) { }
		/*
			   \r\n
			   --myboundary\r\n
			   Content-Type: image/jpeg\r\n
			   Content-Length: 15656\r\n
			   \r\n
			   <JPEG image data>\r\n
			   --myboundary\r\n
			   Content-Type: image/jpeg\r\n
			   Content-Length: 14978\r\n
			   \r\n
			   <JPEG image data>\r\n
			   --myboundary\r\n
			   Content-Type: image/jpeg\r\n
			   Content-Length: 15136\r\n
			   \r\n
			   <JPEG image data>\r\n
			   --myboundary\r\n
				.
				.
				.
				*/
		string Boundary(string content)
		{
			string result = null;
			string extractBoundary = content.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Find(s => s.Contains("boundary"));
			string[] keyValue = extractBoundary.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
			if (keyValue.Length == 2 && keyValue[0] == "boundary")
				result = keyValue[1];
			return result;
		}
		string Parse(byte[] buffer, int start, int length)
		{
			string result = null;
			byte[] selection = new byte[length];
			Array.Copy(buffer, start, selection, 0, length);
			System.Text.StringBuilder b = new System.Text.StringBuilder();
			char[] chars = new System.Text.ASCIIEncoding().GetChars(selection);
			foreach (char v in chars)
				b.Append(v);
			return b.ToString();
		}
		protected override void StreamParser(System.Net.WebResponse response, byte[] buffer, int readSize)
		{

			string boundary = this.Boundary(response.ContentType);
			boundary = boundary.Replace("--", "");

			string separator = "\r\n--" + boundary + "\r\nContent-Type: " + this.Subtype + "\r\nContent-Length: ";
			System.IO.Stream stream = response.GetResponseStream();
			int total = 0;
			int startPosition = 0;
			int endPosition = 0;
			int length = 0;
			bool first = true;
			// loop
			// safe check. Flush the buffer if do not have enough space left.
			while (this.Running)
			{
				if (total + readSize > buffer.Length)
					total = 0;
				int read = 0;
				// read == 0 means end of stream.
				if ((read = stream.Read(buffer, total, readSize)) == 0)
				{
					System.Diagnostics.Debug.WriteLine("End of Stream");
					break;
				}
				total += read;
				if (length == 0 && total > readSize)
				{
					int separatorLength = separator.Length - (first ? 2 : 0);
					string part = new System.Text.StringBuilder().Append(new System.Text.ASCIIEncoding().GetChars(buffer, separatorLength, 20)).ToString();
					int index = part.IndexOf("\r\n\r\n");
					length = Kean.Math.Integer.Parse(part.Substring(0, index));
					startPosition = separatorLength + index + 4; // Start position of data.
					endPosition = length + startPosition;
					first = false;
				}
				if (length != 0 && total > endPosition)
				{
					this.Send(new System.IO.MemoryStream(buffer, startPosition, total));
					int tail = total - endPosition;
					if (tail < endPosition)
						Array.Copy(buffer, endPosition, buffer, 0, tail);
					else
					{
						byte[] temporary = new byte[tail];
						Array.Copy(buffer, endPosition, temporary, 0, tail);
						Array.Copy(temporary, 0, buffer, 0, tail);
					}
					total = tail;
					length = 0;
				}
			}
		}
	}
}
