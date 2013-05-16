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
using Bitmap = Kean.Draw.Raster;
using Kean.Core.Extension;

namespace Imint.Media.DirectShow.Binding.Filters
{
    public abstract class Abstract
    {
        public string Description { get; private set; }
        public Abstract[] Next { get { return this.Followers[0]; } }
        public Abstract[][] Followers { get; private set; }
        public int? Output { get; set; }
        public bool FuzzyMatch { get; set; }
        public Abstract(string description, params Abstract[][] followers)
        {
            this.Description = description;
            this.Followers = followers;
			this.Output = 0;
        }
        public Abstract(string description, params Abstract[] next) :
            this(description, new Abstract[][] { next })
        {
        }
        public override string ToString()
        {
            System.Text.StringBuilder result = new System.Text.StringBuilder(this.Description);
            result.Append("(");
            if (this.Next.NotNull())
                foreach (Abstract candidate in this.Next) // TODO: fix for all followers
                    result.Append(candidate.ToString());
            result.Append("), ");            
            return result.ToString();
        }

        public bool Build(IBuild build)
        {
            return this.Build((DirectShowLib.IPin)null, build);
        }
		public virtual bool Build(DirectShowLib.IBaseFilter previous, IBuild build)
		{
			bool result = false;
			if (this.Output == -1)
			{
				for (int i = 0; i < 6 && !result; i++)
					result = this.Build(previous, i, build);
			}
			else
				result = this.Build(previous, this.Output.Value, build);
			return result;
		}
		public virtual bool Build(DirectShowLib.IBaseFilter previous, int i, IBuild build)
        {
            bool result = false;
            DirectShowLib.IPin outPin = DirectShowLib.DsFindPin.ByDirection(previous, DirectShowLib.PinDirection.Output, i);
			if (outPin.NotNull())
				result = this.Build(outPin, build);
            return result;
        }
        public abstract bool Build(DirectShowLib.IPin source, IBuild build);
    }
}
