// 
//  Network.cs
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
using Kean.Core.Extension;

namespace Imint.Media.DirectShow.Elecard.Timeshift
{

	public class Network :
		DirectShow.NonLinear
	{
		public Network()
		{
		}
		protected override DirectShow.Binding.IGraph Open(Uri.Locator locator)
		{
			Timeshift.Graph.NonLive result = null;
			if (locator.Scheme == "elecard+udp" && locator.Query["video"].NotNull())
			{
				string filename = locator.Query["video"];
				locator = locator.Copy();
				locator.Scheme = "udp";
				locator.Query.Remove("video");
				result = new Graph.NonLive();
				result.Recorder = new DirectShow.Binding.Graph();
				if (result.Recorder.Open(new Filters.Net.SourcePlus(locator, new Filters.File.Sink(filename))))
				{
					result.Recorder.Play();
					System.Threading.Thread.Sleep(1000);
					DirectShow.Binding.Filters.SampleGrabber.All samplegrabber = new DirectShow.Binding.Filters.SampleGrabber.All() { FuzzyMatch = this.Fuzzy };
					bool built = result.Open(new Filters.File.Source(filename, new Filters.Demultiplexer.Mpeg(new Filters.Decoder.All(samplegrabber)) { Output = 0 })) ||
					 result.Open(new Filters.File.Source(filename, new Filters.Demultiplexer.Mpeg(new Filters.Decoder.All(samplegrabber)) { Output = 1 })); 
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
