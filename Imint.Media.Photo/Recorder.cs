// 
//  Recorder.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2011-2014 Imint AB
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
using Kean.Extension;
using Uri = Kean.Uri;
using Geometry2D = Kean.Math.Geometry2D;
using Raster = Kean.Draw.Raster;
using IO = Kean.IO;
using Serialize = Kean.Serialize;

namespace Imint.Media.Photo
{
	public class Recorder :
	IPushRecorder
	{
		long counter;
		Uri.Locator folder;
		string extension;
		TimeSpan lifetime;
		Raster.Compression compression;
		[Serialize.Parameter]
		public bool GenerateScripts { get; set; }
		public Recorder()
		{
		}
		#region IRecorder implementation
		public event Action<Status> StatusChanged;
		Status status;
		public Status Status
		{
			get { return this.status; }
			private set
			{
				if (this.status != value)
					this.StatusChanged.Call(this.status = value);
			}
		}
		public bool Open(Uri.Locator url, Geometry2D.Integer.Size resolution, TimeSpan lifetime)
		{
			bool result = false;
			if (this.Status == Status.Closed)
			{
				switch (this.extension = url.Path.Extension.ToLower())
				{
					case "png":
						this.compression = Raster.Compression.Png;
						result = true;
						break;
					case "jpeg":
					case "jpg":
						this.compression = Raster.Compression.Jpeg;
						result = true;
						break;
					case "bmp":
						this.compression = Raster.Compression.Bmp;
						result = true;
						break;
					case "gif":
						this.compression = Raster.Compression.Gif;
						result = true;
						break;
				}
				if (result)
				{
					this.folder = url.Copy();
					this.folder.Path = this.folder.Path.FolderPath;
					this.lifetime = lifetime;
					this.counter = 0;
					this.Status = Status.Playing;
				}
			}
			return result;
		}
		public bool Close()
		{
			bool result = this.Status != Status.Closed;
			if (result)
			{
				this.Status = Status.Closed;
//				if (this.GenerateScripts)
//				{
//					("ffplay frame%%6d" + "." + this.Extension).Save(this.folder + "play.bat");
//					("ffmpeg " + " -r " + this.fps + " -f image2 -i frame%%6d" + "." + this.Extension + " -sameq " + "\"" + outputFile + "\"").Save(this.folder + "convert.bat");
//				}
			}
			return result;
		}
		#endregion
		#region IPushRecorder implementation
		public bool Append(Raster.Image frame)
		{
			return frame.Save(this.folder + ("frame" + this.counter++.ToString("D6") + "." + this.extension), this.compression);
		}
		#endregion
	}
}

