// 
//  Capture.cs
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
using Kean.Extension;
using Uri = Kean.Uri;
using Serialize = Kean.Serialize;
using Geometry2D = Kean.Math.Geometry2D;
using Math = Kean.Math;
using Collection = Kean.Collection;

namespace Imint.Media.DirectShow
{
	public class Capture :
		Stream,
		Player.ICapture
	{
		[Serialize.Parameter]
		public Geometry2D.Integer.Shell Crop { get; set; }
		[Serialize.Parameter]
		public Math.Fraction Ratio { get; set; }
		[Serialize.Parameter]
		public Collection.List<string> Blacklist { get; private set; }

		public System.Collections.Generic.IEnumerable<Resource> Devices
		{
			get 
			{
				Uri.Query query = new Uri.Query();
				if (this.Crop.NotZero)
					query["crop"] = this.Crop;
				if (this.Ratio.Nominator != 0)
					query["ratio"] = this.Ratio;
				foreach (string device in DirectShow.Binding.Graph.Devices)
					if (!this.Blacklist.Contains(device))
						yield return new Resource(ResourceType.Capture, device, new Uri.Locator()
						{
							Scheme = "directshow+capture",
							Authority = device,
							Query = query,
						});
			}
		}

		protected override DirectShow.Binding.IGraph Open(Uri.Locator locator)
		{
			DirectShow.Binding.IGraph result = null; 
			if (locator.Scheme == "directshow+capture" && locator.Authority.NotNull())
			{
				result = new DirectShow.Binding.Graph();
				bool built = result.Open(new DirectShow.Binding.Filters.Capture.All(locator.Authority, new DirectShow.Binding.Filters.SampleGrabber.Yuyv(new DirectShow.Binding.Filters.NullRenderer()) { FuzzyMatch = this.Fuzzy }));
				if (built)
					result.Play();
				else
				{
					result.Close();
					result = null;
				}
			}
			return result;
		}
	}
}
