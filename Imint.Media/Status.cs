// 
//  Status.cs
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

namespace Imint.Media
{
	/// <summary>
	/// Used to signal the status of the player.
	/// </summary>
	public enum Status
	{
		/// <summary>
		/// The player is closed and no input source is opened right now.
		/// </summary>
		Closed,
		/// <summary>
		/// A source is opened but it is not playing.
		/// </summary>
		Paused,
		/// <summary>
		/// A source is opened and it plays.
		/// </summary>
		Playing,
	}
}
