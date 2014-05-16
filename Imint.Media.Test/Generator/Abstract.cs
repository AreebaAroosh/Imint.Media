// 
//  Abstract.cs
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

using Kean;
using Kean.Extension;
using System;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Collection;
using Uri = Kean.Uri;
using Serialize = Kean.Serialize;
using Parallel = Kean.Parallel;
using Error = Kean.Error;

namespace Imint.Media.Test.Generator
{
	public abstract class Abstract :
		Collection.Abstract.ReadOnlyVector<Tuple<Raster.Image, Tuple<string, object>[]>>,
		IDisposable
	{
		public abstract string Name { get; }
		Collection.IList<KeyValue<string, Uri.Locator>> devices = new Collection.List<KeyValue<string, Uri.Locator>>();
		[Serialize.Parameter("Device")]
		public Collection.IList<KeyValue<string, Uri.Locator>> Devices { get { return this.devices; } }
		protected Abstract()
		{
		}
		~Abstract()
		{
			Error.Log.Call(((IDisposable)this).Dispose);
		}
		public abstract void Open(Uri.Locator argument, Parallel.ThreadPool threadPool);
		public virtual void Close()
		{
			foreach (Tuple<Raster.Image, Tuple<string, object>[]> item in this)
				item.Item1.TryDispose();
		}

		void IDisposable.Dispose()
		{
			this.Close();
		}
	}
}
