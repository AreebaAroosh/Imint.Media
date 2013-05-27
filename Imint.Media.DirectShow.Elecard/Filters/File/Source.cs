// 
//  Source.cs
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
using Error = Kean.Core.Error;
using Kean.Core.Extension;
using Kean.Core;

namespace Imint.Media.DirectShow.Elecard.Filters.File
{
    public class Source :
       Abstract
    {
        string file;
		public Source(string file, params DirectShow.Binding.Filters.Abstract[] next) :
			base("file.source", new System.Guid(global::Elecard.ElUids.Filters.CLSID_FileListSource), "efls.ax", "Elecard File List Source Filter \"" + file + "\"", next)
        {
            this.file = file;
            System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file));
            this.Output = 0;
        }
		public override DirectShowLib.IBaseFilter Create()
		{
			this.Configure(new string[] { 
                "Software", 
                "Elecard", 
                "Elecard File List Source", 
                System.IO.Path.GetFileName(System.Environment.GetCommandLineArgs()[0]) },
				KeyValue.Create("Dynamic length", 1)
				);
			DirectShowLib.IBaseFilter result = base.Create();
			if (result is DirectShowLib.IFileSourceFilter)
				Binding.Exception.GraphError.Check((result as DirectShowLib.IFileSourceFilter).Load(this.file, new DirectShowLib.AMMediaType() { majorType = DirectShowLib.MediaType.Stream, subType = DirectShowLib.MediaSubType.Mpeg2Transport }));
			return result;
		}
		public override bool Build(DirectShowLib.IPin source, Imint.Media.DirectShow.Binding.IBuild build)
		{
			bool result = false;
			DirectShowLib.IBaseFilter filter = this.Create();
			if (filter.NotNull() && build.Graph.AddFilter(filter, this.Description) == 0)
			{
				foreach (DirectShow.Binding.Filters.Abstract candidate in this.Next)
					if (result = candidate.Build(filter, build))
						break;
			}
			else
			{
				Error.Log.Append(Error.Level.Debug, "Unable to open file.", "DirectShow was unable to open file \"" + this.file + "\".");
				Binding.Exception.GraphError.Check(build.Graph.RemoveFilter(filter));
			}
			return result;
		}
    }
}
