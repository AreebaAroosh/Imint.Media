// 
//  File.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2012-2013 Imint AB
// 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//  the documentation and/or other materials provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
using System;
using Uri = Kean.Core.Uri;
using Bitmap = Kean.Draw.Raster;
using Kean.Core.Extension;
using DirectShow = Imint.Media.DirectShow;
using Kean.Core.Collection.Extension;

namespace Imint.Media.DirectShow.CyberLink
{
	public class File :
		DirectShow.NonLinear,
		Media.Player.IFile
	{
		public File()
		{ }
		protected override DirectShow.Binding.IGraph Open(Uri.Locator name)
		{
			DirectShow.Binding.IGraph result = null;
			if (name.Path.NotNull())
			{
				string file = name.Path.PlatformPath;
				string extension = System.IO.Path.GetExtension(file).ToLower().TrimStart('.');
				if (name.Scheme == "file" && file.NotEmpty() && this.SupportedExtensions.Exists(v => v == extension) && System.IO.File.Exists(file))
				{
					result = new Binding.Graph(this.Application);
					if (!this.Open(result, file))
					{
						result.Close();
						result = null;
					}
				}
			}
			return result;
		}
		bool Open(DirectShow.Binding.IGraph graph, string file)
		{
			return graph.Open(new DirectShow.Binding.Filters.File.Source(file, new Filters.Demultiplexer.Mpeg(new Filters.Decoder.Mpeg(new DirectShow.Binding.Filters.SampleGrabber.All()))));
		}
		#region IFile Members
		public string[] SupportedExtensions
		{
			get { return new string[] { "avi", "mpg" }; }
		}
		#endregion
	}
}
