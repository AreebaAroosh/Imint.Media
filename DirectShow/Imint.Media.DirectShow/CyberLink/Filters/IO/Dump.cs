﻿// 
//  Dump.cs
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
using Error = Kean.Error;
using Kean.Extension;

namespace Imint.Media.DirectShow.CyberLink.Filters.IO
{
	public class Dump :
		DirectShow.Binding.Filters.FromFile
	{
		string file;
		public Dump(string file, params DirectShow.Binding.Filters.Abstract[] next) :
			base(new System.Guid("3EE61520-7422-4565-912B-8C7D859EFD24"), "CLDump.ax", "PP Dump Filter \"" + file + "\"", next)
		{
			this.file = file;
			System.IO.Directory.CreateDirectory(System.IO.Path.GetDirectoryName(file));
		}
		public override DirectShowLib.IBaseFilter Create()
		{
			DirectShowLib.IBaseFilter result = base.Create();
			if (result is DirectShowLib.IFileSinkFilter)
			{
				Binding.Exception.GraphError.Check((result as DirectShowLib.IFileSinkFilter).SetFileName(this.file, new DirectShowLib.AMMediaType() { majorType = DirectShowLib.MediaType.Stream, subType = DirectShowLib.MediaSubType.Mpeg2Program }));
			}
			return result;
		}
	}
}
