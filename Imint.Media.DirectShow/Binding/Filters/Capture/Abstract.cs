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
using Kean.Extension;
using Geometry2D = Kean.Math.Geometry2D;
using Collection = Kean.Collection;
using Buffer = Kean.Buffer;
using Kean.Collection.Extension;
using Error = Kean.Error;

namespace Imint.Media.DirectShow.Binding.Filters.Capture
{
	public abstract class Abstract :
		Filters.Abstract
	{
		string device;
		public Abstract(string device, params Filters.Abstract[] next) :
			base(device, next)
		{
			this.device = device;
			this.Output = 0;
		}
		public override bool Build(DirectShowLib.IPin source, IBuild build)
		{
			bool result = false;
			if (Abstract.FindCaptureDeviceNameByIdentifier(this.device).NotNull())
			{
				DirectShowLib.IBaseFilter filter = Abstract.FindCaptureDeviceByIdentifier(this.device, false);
				if (filter.NotNull() && this.SetFormat(filter))
				{
					if (build.Graph.AddFilter(filter, "Capture") == 0)
					{
						foreach (Filters.Abstract candidate in this.Next)
							if (result = candidate.Build(filter, 0, build))
								break;
					}
					else
					{
						Error.Log.Append(Error.Level.Debug, "Unable to open capture.", "DirectShow was unable to capture \"" + this.device + "\".");
						Exception.GraphError.Check(build.Graph.RemoveFilter(filter));
					}
				}
			}
			return result;
		}
		DirectShowLib.AMMediaType[] GetOutputMediaTypes(DirectShowLib.IBaseFilter filter)
		{
			DirectShowLib.AMMediaType[] result = null;
			DirectShowLib.IPin outPin = DirectShowLib.DsFindPin.ByDirection(filter, DirectShowLib.PinDirection.Output, 0);
			if (outPin is DirectShowLib.IAMStreamConfig)
			{
				int count = 0;
				int size = 0;
				Exception.GraphError.Check((outPin as DirectShowLib.IAMStreamConfig).GetNumberOfCapabilities(out count, out size));
				Buffer.Vector<byte> buffer = new Buffer.Vector<byte>(size);
				result = new DirectShowLib.AMMediaType[count];
				for (int i = 0; i < count; i++)
					Exception.GraphError.Check((outPin as DirectShowLib.IAMStreamConfig).GetStreamCaps(i, out result[i], buffer));
			}
			return result;
		}
		void SetOutputMedia(DirectShowLib.IBaseFilter filter, DirectShowLib.AMMediaType media)
		{
			DirectShowLib.IPin outPin = DirectShowLib.DsFindPin.ByDirection(filter, DirectShowLib.PinDirection.Output, 0);
			if (outPin is DirectShowLib.IAMStreamConfig)
				Exception.GraphError.Check((outPin as DirectShowLib.IAMStreamConfig).SetFormat(media));
		}
		bool SetFormat(DirectShowLib.IBaseFilter filter)
		{
			bool result = false;
			Format.Image wanted = this.Media(filter);
			if (wanted.NotNull())
			{
				if (wanted.Resolution.NotNull())
				{
					DirectShowLib.AMMediaType[] media = this.GetOutputMediaTypes(filter);
					if (media.NotNull())
					{
						for (int i = 0; i < media.Length; i++)
						{
							DirectShowLib.VideoInfoHeader header = Abstract.GetHeader(media[i]);
							if (media[i].subType == wanted.Type &&
								header.BmiHeader.Width == wanted.Resolution.Width)
							{
								if (header.BmiHeader.Height == wanted.Resolution.Height)
								{
									this.SetOutputMedia(filter, media[i]);
									result = true;
									break;
								}
								else if (wanted.ForceHeight)
								{
									header.BmiHeader.Height = wanted.Resolution.Height;
									header.BmiHeader.ImageSize = header.BmiHeader.Width * header.BmiHeader.Height * header.BmiHeader.BitCount / 8;
									header.BitRate = (int)((10000000 / header.AvgTimePerFrame) * header.BmiHeader.ImageSize * 8);
									Abstract.SetHeader(media[i], header);
									media[i].sampleSize = header.BmiHeader.ImageSize;
									this.SetOutputMedia(filter, media[i]);
									result = true;
									break;
								}
							}
						}
					}
				}
				else
					result = true;
			}
			return result;
		}
		protected virtual Format.Image Media(DirectShowLib.IBaseFilter filter)
		{
			return new Format.Image();
		}
		protected static DirectShowLib.VideoInfoHeader GetHeader(DirectShowLib.AMMediaType media)
		{
			return System.Runtime.InteropServices.Marshal.PtrToStructure(media.formatPtr, typeof(DirectShowLib.VideoInfoHeader)) as DirectShowLib.VideoInfoHeader;
		}
		protected static void SetHeader(DirectShowLib.AMMediaType media, DirectShowLib.VideoInfoHeader header)
		{
			System.Runtime.InteropServices.Marshal.StructureToPtr(header, media.formatPtr, true);
		}

		public static DirectShowLib.IBaseFilter FindCaptureDeviceByIdentifier(string identifier, bool defaultFirstDevice)
		{
			DirectShowLib.DsDevice[] devices = DirectShowLib.DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
			object source = null;

			// Take the first device
			DirectShowLib.DsDevice device = defaultFirstDevice ? devices[0] : null;

			foreach (DirectShowLib.DsDevice dev in devices)
			{
				if (dev.DevicePath.Equals(identifier) || dev.Name.Equals(identifier))
				{
					device = dev;
					break;
				}
			}

			// Bind Moniker to a filter object
			if (device.NotNull())
			{
				System.Guid iid = typeof(DirectShowLib.IBaseFilter).GUID;
				device.Mon.BindToObject(null, null, ref iid, out source);
			}

			// An exception is thrown if cast fail
			return (DirectShowLib.IBaseFilter)source;
		}
		public static string FindCaptureDeviceNameByIdentifier(string identifier)
		{
			DirectShowLib.DsDevice[] devices = DirectShowLib.DsDevice.GetDevicesOfCat(DirectShowLib.FilterCategory.VideoInputDevice);
			string deviceName = null;

			foreach (DirectShowLib.DsDevice dev in devices)
				if (dev.DevicePath.Equals(identifier) || dev.Name.Equals(identifier))
				{
					deviceName = dev.Name;
					break;
				}

			return deviceName;
		}

	}
}
