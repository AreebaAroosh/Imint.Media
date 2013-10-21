// 
//  Source.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2013 Imint AB
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
using Kean;
using Kean.Extension;
using Uri = Kean.Uri;
using Geometry2D = Kean.Math.Geometry2D;
using Draw = Kean.Draw;
using Collection = Kean.Collection;

namespace Imint.Media
{
	public class Resource
	{
		public ResourceType Type { get; private set; }
		string label;
		public string Label { get { return this.label ?? (this.Locator.NotNull() ? (string)this.Locator.Authority : "(unnamed)"); } }
		public Uri.Locator Locator { get; private set; }
		public Resource(ResourceType type, string label, Uri.Locator locator)
		{
			this.Type = type;
			this.label = label;
			this.Locator = locator;
		}
		public static implicit operator string(Resource resource)
		{
			return resource.Type.AsString() + ", " + resource.Label + ", " + resource.Locator;
		}
		public static explicit operator Resource(string resource)
		{
			Resource result = null;
			if (resource.NotEmpty())
			{
				string[] splitted = resource.Split(',');
				int c = 0;
				int length = splitted.Length;
				result = new Resource(length > 2 ? splitted[c++].Parse<ResourceType>() : ResourceType.Unknown, length > 1 ? splitted[c++].Trim() : null, splitted[c]);
			}
			return result;
		}
	}
}
