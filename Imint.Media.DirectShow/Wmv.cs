// 
//  Wmv.cs
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
using Uri = Kean.Uri;

namespace Imint.Media.DirectShow
{
	public class Wmv :
		File
	{
		protected override bool Open(Binding.IGraph graph, string file)
		{
			bool result = false;
			DirectShow.Binding.Filters.SampleGrabber.All samplegrabber = new DirectShow.Binding.Filters.SampleGrabber.All() { Rate = this.Rate, FuzzyMatch = this.Fuzzy };
			if (this.Fuzzy)
				result = graph.Open(new DirectShow.Binding.Filters.File.AsfReader(file, samplegrabber));
			else
			{
				result = graph.Open(new DirectShow.Binding.Filters.File.AsfReader(file, new DirectShow.Binding.Filters.Decoder.Wmv(samplegrabber) { Output = 0 })) ||
				graph.Open(new DirectShow.Binding.Filters.File.AsfReader(file, new DirectShow.Binding.Filters.Decoder.Wmv(samplegrabber) { Output = 1 })) ||
				graph.Open(new DirectShow.Binding.Filters.File.AsfReader(file, new DirectShow.Binding.Filters.Decompressor.Mjpeg(samplegrabber) { Output = 0 })) ||
				graph.Open(new DirectShow.Binding.Filters.File.AsfReader(file, new DirectShow.Binding.Filters.Decompressor.Mjpeg(samplegrabber) { Output = 1 }));
			}
			return result;
		}
		public override string[] SupportedExtensions
		{
			get { return new string[] { "wmv", "asf" }; }
		}
	}
}
