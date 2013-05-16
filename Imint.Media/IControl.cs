// 
//  IControl.cs
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
using Uri = Kean.Core.Uri;
using Geometry2D = Kean.Math.Geometry2D;

namespace Imint.Media
{
    public interface IControl :
		IDisposable
    {
        Uri.Locator Resource { get; }
        event Action<Uri.Locator> ResourceChanged;
        Status Status { get; }
        event Action<Status> StatusChanged;
		DateTime Start { get; }
        event Action<DateTime> StartChanged;
        DateTime Position { get; }
        event Action<DateTime> PositionChanged;
        DateTime End { get; }
        event Action<DateTime> EndChanged;

        bool Seekable { get; }
        event Action<bool> SeekableChanged;
        bool HasNext { get; }
        event Action<bool> HasNextChanged;
        bool HasPrevious { get; }
        event Action<bool> HasPreviousChanged;

        string[] Extensions { get; }

		bool Open(Uri.Locator resource);
        void Play();
        void Pause();
        void Eject();
        void Seek(DateTime position);
        void Next();
        void Previous();
	}
}
