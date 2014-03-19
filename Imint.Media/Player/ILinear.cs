// 
//  ILinear.cs
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

namespace Imint.Media.Player
{
	/// <summary>
	/// Interface to fulfill for players supporting play, pause and position functionality.
	/// This is intended for file formats lacking seek functionality, for example when an index is missing.
	/// If seek and total length is available use INonLinear instead and if pause is not available use IStream.
	/// If the player opens file resources implement IFile as well.
	/// All calls to the player starting with open and until a close is done is guaranteed to be done from the same thread.
	/// </summary>
	public interface ILinear :
		IStream
	{
		/// <summary>
		/// Returns false if the opened resource does not support the ILinear capabilities although the player does implement them.
		/// </summary>
		bool IsLinear { get; }
		/// <summary>
		/// Timestamp of the current position. This shall preferably indicate the real date and time when the 
		/// current frame was recorded but it may contain elapsed time since start of the video nothing else is available.
		/// </summary>
		DateTime Position { get; }
		/// <summary>
		/// Called to start playing of resource.
		/// </summary>
		/// <returns>True if resource started to play.</returns>
		bool Play();
		/// <summary>
		/// Called to halt playing of resource.
		/// </summary>
		/// <returns>True if pause was successful.</returns>
		bool Pause();
	}
}
