﻿// 
//  CaptureTimeshift.cs
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
using Kean.Extension;

namespace Imint.Media.DirectShow.CyberLink
{

	public class CaptureTimeshift :
		DirectShow.NonLinear
	{
		public CaptureTimeshift()
		{ }
		protected override DirectShow.Binding.IGraph Open(Uri.Locator name)
		{
			Graph.Live result = null;
			if (name.Scheme == "directshow+capture" && name.Authority.NotNull() && name.Query.NotNull() && name.Query["video"].NotNull())
			{
				DirectShow.Binding.Graph temporary = new DirectShow.Binding.Graph();
				bool built = temporary.Open(new DirectShow.Binding.Filters.Capture.All(name.Authority, new DirectShow.Binding.Filters.SampleGrabber.Yuyv(new DirectShow.Binding.Filters.NullRenderer()) { FuzzyMatch = this.Fuzzy }));
				if (built)
				{
					temporary.Play();
					System.Threading.Thread.Sleep(500);
				}
				temporary.Close();
				temporary = null;
				string filename = name.Query["video"];
				result = new Graph.Live();
				result.Recorder = new DirectShow.Binding.Graph();
				if (result.Recorder.Open(new DirectShow.Binding.Filters.Capture.All(name.Authority, new DirectShow.Binding.Filters.SampleGrabber.Yuyv(new Filters.Encoder.Mpeg(new Filters.Multiplexer.Mpeg(new Filters.IO.Dump(filename)))))))
				{
					result.Recorder.Play();
					System.Threading.Thread.Sleep(2000);
					built = result.Open(new Filters.IO.Reader(filename, new Filters.Demultiplexer.Mpeg(new Filters.Decoder.Mpeg(new DirectShow.Binding.Filters.SampleGrabber.All()))));
					if (built)
					{
						System.Threading.Thread.Sleep(1000);
						result.Play();
					}
					else
					{
						result.Close();
						result = null;
					}
				}
				else
				{
					result.Close();
					result = null;
				}
			}
			return result as DirectShow.Binding.IGraph;
		}
	}
}



