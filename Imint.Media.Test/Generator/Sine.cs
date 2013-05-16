// 
//  Sine.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2010-2013 Imint AB
// 
//  This program is free software: you can redistribute it and/or modify
//  it under the terms of the GNU Lesser General Public License as published by
//  the Free Software Foundation, either version 3 of the License, or
//  (at your option) any later version.
// 
//  This program is distributed in the hope that it will be useful,
//  but WITHOUT ANY WARRANTY; without even the implied warranty of
//  MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//  GNU Lesser General Public License for more details.
// 
//  You should have received a copy of the GNU Lesser General Public License
//  along with this program.  If not, see <http://www.gnu.org/licenses/>.

using System;
using Kean.Core;
using Bitmap = Kean.Draw.Raster;
using Geometry2D = Kean.Math.Geometry2D;
using Uri = Kean.Core.Uri;
using Kean.Core.Collection.Extension;
using Kean.Core.Extension;
using Kean.Core.Collection.Linked.Extension;
using Serialize = Kean.Core.Serialize;

namespace Imint.Media.Test.Generator
{
    public class Sine :
        Cached
    {
        int frames;
        Geometry2D.Integer.Size resolution;
        public Sine()
        {
            this.Initialize();
        }
        [Serialize.Parameter]
        public Colorspace Format { get; set; }
        public override string Name
        {
            get { return "sine"; }
        }
        protected override int Prepare(Uri.Locator argument)
        {
            try
            {
				if (!argument.Query.Empty)
				{
					string value = argument.Query["resolution"];
					if (value.NotEmpty())
						this.resolution = (Geometry2D.Integer.Size)value;
					value = argument.Query["format"];
					if (value.NotEmpty())
						base.Format = (Colorspace)Enum.Parse(typeof(Colorspace), value, true);
					value = argument.Query["frames"];
					if (value.NotEmpty())
						this.frames = Kean.Math.Integer.Parse(value);
				}
            }
            catch (Exception e)
            {
                this.Initialize();
            }
            return this.frames;
        }
        protected override Tuple<Bitmap.Image, Tuple<string, object>[]> Generate(int frame)
        {
            Bitmap.Image result = new Bitmap.Bgra(this.resolution);
            int width = result.Size.Width;
            unsafe
            {
                byte* pointer = (byte*)result.Pointer;
				for (int x = 0; x < result.Size.Width; x++)
					for (int y = 0; y < result.Size.Height; y++)
                    {
						int pixel = (x + y * result.Size.Width) * 4;
                        double value =
							(Math.Sin(Convert.ToSingle((x + frame * 10) % result.Size.Width) / Convert.ToSingle(result.Size.Width) * 2.0 * Math.PI * 7) +
							Math.Sin(Convert.ToSingle(y) / Convert.ToSingle(result.Size.Height) * 2.0 * Math.PI * 5) + 2.0)
                            / 4.0 * 254.0;
                        byte byteValue = Convert.ToByte(value * (Math.Sin(Convert.ToSingle(y * 0.005 + frame * 0.5)) + 1.0) / 2.0);
                        byte red = byteValue;
                        byte green = 0;
                        byte blue = Convert.ToByte(255 - byteValue);
                        pointer[4 * (y * width + x) + 0] = blue;
                        pointer[4 * (y * width + x) + 1] = green;
                        pointer[4 * (y * width + x) + 2] = red;
                        pointer[4 * (y * width + x) + 3] = 255;
                    }
            }
            return Tuple.Create<Bitmap.Image, Tuple<string, object>[]>(result, null);
        }
        private void Initialize()
        {
            this.frames = 10;
            this.resolution = new Geometry2D.Integer.Size(640, 480);
            base.Format = Colorspace.Bgra;
        }
    }
}
