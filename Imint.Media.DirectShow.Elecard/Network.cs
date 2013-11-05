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
using Serialize = Kean.Core.Serialize;

namespace Imint.Media.DirectShow.Elecard
{
	public class Network :
		DirectShow.Stream,
		Media.Player.ICapture
	{
		/// <summary>
		/// Time, in seconds, after which to stop waiting for a stream to send more data.
		/// Set to 0 to never give up, recovering as soon as the stream comes back online.
		/// Set to 1 to give up after 1 second. Set to 300 to give up after 5 minutes.
		/// Default is 0.
		/// </summary>
		[Serialize.Parameter]
		public int Timeout { get; set; }
		/// <summary>
		/// Number of frames to allow the stream to buffer before playing.
		/// If the buffer is too small, the video may stutter and garble.
		/// Set to 0 for no latency. Set to 1 or higher if your machine can't keep up.
		/// Default is 0.
		/// </summary>
		[Serialize.Parameter]
		public int Latency { get; set; }

		protected override DirectShow.Binding.IGraph Open(Uri.Locator name)
		{
			DirectShow.Binding.IGraph result = null;
			if (name.Scheme.Head == "elecard" && name.Scheme.Tail.NotNull() && name.Scheme.Tail.Head != "file" && name.Authority.NotNull() && name.Query["video"].IsNull())
			{
				name = name.Copy();
				this.Timeout = name.Query.Get("timeout", this.Timeout);
				this.Latency = name.Query.Get("latency", this.Latency);
				name.Query.Remove("timeout", "latency");
				name.Scheme = name.Scheme.Tail;
				result = new DirectShow.Binding.Graph(this.Application);
				if (this.Open(result, name))
				{
					result.Play();
					result.Stop();
					result.Play();
				}
				else
				{
					result.Close();
					result = null;
				}
			}
			return result;
		}
		bool Open(DirectShow.Binding.IGraph graph, Uri.Locator name)
		{
			return graph.Open(new Filters.Net.SourcePlus(name, new Filters.Demultiplexer.MpegPush(new Filters.Decoder.All(new DirectShow.Binding.Filters.SampleGrabber.All()) { Output = -1 }) { WaitForOutput = new TimeSpan(0, 0, 0, 1), Latency = this.Latency }) { Timeout = this.Timeout }) ||
			graph.Open(new Filters.Net.RtspSource(name, new Filters.Decoder.SD.Avc(new DirectShow.Binding.Filters.SampleGrabber.All())));
		}

		public System.Collections.Generic.IEnumerable<Resource> Devices
		{
			// TODO: Enumerate network streams
			get { yield break; }
		}
	}
}
