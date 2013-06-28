// 
//  INonLinear.cs
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
	/// Interface to fullfill when player has seek functionality.
	/// For less functionality use ILinear or IStream instead.
	/// If the player opens file resources implement IFile as well.
	/// All calls to the player starting with open and until a close is done is guaranteed to be done from the same thread.
	/// </summary>
	public interface INonLinear :
		ILinear
	{
		/// <summary>
		/// Returns false if the opened resource does not support the INonLinear capabilities although the player does implement them.
		/// If it returns true IsLinear must also return true since INonLinear is a superset of ILinear.
		/// </summary>
		bool IsNonLinear { get; }
		/// <summary>
		/// The time and date at which recording of the video started. ILinear.Postion is relative to this.
		/// If this is not available it should be zero and the position should be relative to zero.
		/// </summary>
		DateTime Start { get; }
		/// <summary>
		/// The time and date when the recording of the video ended. If the recording is still performed this value will contain current 
		/// date and time and will be changing.
		/// </summary>
		DateTime End { get; }
		/// <summary>
		/// Method that initiates a seek in the video stream.
		/// Upon return the seek must not have been perfomed yet. If the desired position is unreachable a for the situation
		/// relevant position should be used instead.
		/// </summary>
		/// <param name="position">Position in the range between Start and End to which to seek.</param>
		void Seek(DateTime position);
	}
}
