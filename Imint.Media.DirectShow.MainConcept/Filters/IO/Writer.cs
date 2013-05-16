// 
//  Writer.cs
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
using Kean.Core.Extension;

namespace Imint.Media.DirectShow.MainConcept.Filters.IO
{
    public class Writer :
        DirectShow.Binding.Filters.FromFile
    {
        string filename;
        public Writer(string filename) :
            base(new System.Guid("56ECEE04-1726-4D6F-8792-21C8DBACA7BE"), "HCWTSWriter.ax", "Hauppauge Transport Writer")
        {
            this.filename = filename;
        }
        public override DirectShowLib.IBaseFilter Create()
        {
            DirectShowLib.AMMediaType media = new DirectShowLib.AMMediaType() { majorType = DirectShowLib.MediaType.Stream, subType = DirectShowLib.MediaSubType.Mpeg2Program };
            DirectShowLib.IBaseFilter result = base.Create();
            if (result.NotNull())
            {
                DirectShowLib.FilterInfo info;
                result.QueryFilterInfo(out info);
                string info2;
                result.QueryVendorInfo(out info2);
                DirectShow.Binding.Exception.GraphError.Check((result as DirectShowLib.IFileSinkFilter).SetFileName(this.filename, media));
            }
            return result;
        }
        public override bool Build(DirectShowLib.IPin source, DirectShow.Binding.IBuild build)
        {
            build.Playing += () =>
            {
            //    if (this.recorder.NotNull())
            //        this.recorder.StartBackupToFile(this.filename, 0);
            };
            build.OnClose += () =>
            {
            //    if (this.recorder.NotNull())
            //        this.recorder.StopBackupToFile();
            };
            return base.Build(source, build);
        }
    }
}
