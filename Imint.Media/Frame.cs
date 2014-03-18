// 
//  Frame.cs
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
using Kean;
using Kean.Extension;
using Geometry2D = Kean.Math.Geometry2D;
using Draw = Kean.Draw;
using Collection = Kean.Collection;

namespace Imint.Media
{
	public class Frame
	{
		public int Channel { get; set; }
		public DateTime Time { get; set; }
		public TimeSpan Elapsed { get; set; }
		public TimeSpan Lifetime { get; set; }
		public Func<bool, Frame> Done { get; set; }
		public float Ratio { get; set; }
		public Scan Scan { get; set; }
		Draw.Image content;
		public Draw.Image Content 
		{
			get { return this.content; }
			set
			{
				this.content = value;
				if (this.content.NotNull())
					this.ReferenceCount = new ReferenceCounter(this.content);
			}
		}
		public ReferenceCounter ReferenceCount { get; set; }
		public Collection.IReadOnlyDictionary<string, object> Meta { get; set; }
		public Frame()
		{ }
		public Frame(int channel, DateTime time, TimeSpan lifeTime, Draw.Image content, params Tuple<string, object>[] meta) :
			this(channel, time, lifeTime, content, meta.Map(m => KeyValue.Create(m.Item1, m.Item2)))
		{ }

		public Frame(int channel, DateTime time, TimeSpan lifeTime, Draw.Image content, params KeyValue<string, object>[] meta)
		{
			this.Channel = channel;
			this.Time = time;
			this.Lifetime = lifeTime;
			this.Content = content;
			this.Meta = new Collection.ReadOnlyDictionary<string, object>(meta ?? new KeyValue<string, object>[0]);
		}
	}
}
