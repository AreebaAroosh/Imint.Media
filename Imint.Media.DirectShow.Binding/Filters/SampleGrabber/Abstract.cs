// 
//  Abstract.cs
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
using Bitmap = Kean.Draw.Raster;
using Buffer = Kean.Core.Buffer;
using Geometry2D = Kean.Math.Geometry2D;
using Kean.Core.Extension;

namespace Imint.Media.DirectShow.Binding.Filters.SampleGrabber
{
	public abstract class Abstract :
		Creator,
		IDisposable
	{
		public Kean.Math.Fraction Rate { get; set; }
		public Kean.Draw.CoordinateSystem CoordinateSystem { get; set; }
		IBuild build;
		Geometry2D.Integer.Size size;
		long lifetime;
		DirectShowLib.ISampleGrabber grabber;
		protected abstract System.Guid SubType { get; }
		protected Abstract(string description, params Filters.Abstract[] next) :
			base(description, next)
		{
			this.Output = 0;
		}
		
		public override DirectShowLib.IBaseFilter Create()
		{
			DirectShowLib.ISampleGrabber result = new DirectShowLib.SampleGrabber() as DirectShowLib.ISampleGrabber;
			Exception.GraphError.Check(result.SetMediaType(new DirectShowLib.AMMediaType() { majorType = DirectShowLib.MediaType.Video, formatType = DirectShowLib.FormatType.VideoInfo, subType = this.SubType }));
			Exception.GraphError.Check(result.SetBufferSamples(true));
			Exception.GraphError.Check(result.SetCallback(new Callback(this), 1));
			return (this.grabber = result) as DirectShowLib.IBaseFilter;
		}
		public override bool Build(DirectShowLib.IPin source, IBuild build)
		{
			this.build = build;
			this.build.OnClose += this.Dispose;
			bool result;
			if (result = base.Build(source, build))
			{
				DirectShowLib.AMMediaType media = new DirectShowLib.AMMediaType();
				Exception.GraphError.Check((this.grabber as  DirectShowLib.ISampleGrabber).GetConnectedMediaType(media));
				DirectShowLib.VideoInfoHeader header = (DirectShowLib.VideoInfoHeader)System.Runtime.InteropServices.Marshal.PtrToStructure(media.formatPtr, typeof(DirectShowLib.VideoInfoHeader));
				this.size = new Geometry2D.Integer.Size(header.BmiHeader.Width, header.BmiHeader.Height);
				this.lifetime = header.AvgTimePerFrame;
				// NOTE!!!! Here we set a default frame rate if the video does not have such information available.
				if (this.lifetime < 1000 || this.lifetime > 10000000)
					this.lifetime = 400000;
				if (this.Rate.NotNull())
				{
					double factor = (double)this.Rate / (1000 / new TimeSpan(this.lifetime).TotalMilliseconds) ;
					int code = (this.build.Graph as DirectShowLib.IMediaSeeking).SetRate(factor);
				}
			}
			return result;
		}
		protected abstract Bitmap.Image CreateBitmap(Buffer.Sized data, Geometry2D.Integer.Size resolution);
		~Abstract()
		{
			Error.Log.Wrap((Action)this.Dispose)();
		}
		public void Dispose()
		{
			if (this.build.NotNull())
			{
				this.build.OnClose -= this.Dispose;
				this.build = null;
			}
			if (this.grabber.NotNull())
			{
				this.grabber.SetCallback(null, 1);
				this.grabber = null;
			}
		}
		#region Callback
		class Callback :
			DirectShowLib.ISampleGrabberCB
		{
			Abstract parent;
			public Callback(Abstract parent)
			{
				this.parent = parent;
			}
			#region ISampleGrabberCB Members
			int DirectShowLib.ISampleGrabberCB.BufferCB(double position, IntPtr buffer, int length)
			{
				this.parent.build.Send.Call(new TimeSpan((long)(position * 1000 * 10000)), new TimeSpan(this.parent.lifetime), this.parent.CreateBitmap(new Buffer.Sized(buffer, length), this.parent.size));
				return 0;
			}
			int DirectShowLib.ISampleGrabberCB.SampleCB(double SampleTime, DirectShowLib.IMediaSample pSample)
			{
				throw new NotImplementedException();
			}
			#endregion
		}
		#endregion
	}
}
