// 
//  IStream.cs
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
using Raster = Kean.Draw.Raster;
using Uri = Kean.Uri;

namespace Imint.Media.Player
{
	/// <summary>
	/// Primary interface for players of video and meta data content.
	/// This interface does not provide any play, pause or seek functionality. 
	/// It is intended for simple capture device input without any recording or buffering.
	/// When more functionality is required use ILinear or INonLinear instead.
	/// If the player has the capability to open files make sure to implement IFile as well.
	/// All calls to the player starting with open and until a close is done is guaranteed to be done from the same thread.
	/// </summary>
	public interface IStream :
		IDisposable
	{
		/// <summary>
		/// Number of channels used by this player.
		/// </summary>
		int Channels { get; }
		/// <summary>
		/// Method called by the input thread to poll for new frames. 
		/// This method is where the input thread is idle and it needs therefore to contain some kind of sleep or wait.
		/// </summary>
		void Poll();
		/// <summary>
		/// Callback used when player has a new frame. The incoming frame data will be copied to a new location during this method.
		/// Arguments:
		/// int channel, which output shall the data be sent on.
		/// DateTime timestamp, timestamp of the frame.
		/// Timespan lifetime, indication of how long this frame is valid
		/// Bitmap.Abstract bitmap, bitmap data of the frame.
		/// Tuple<string, object>[] meta, array with meta data objects and their keys.
		/// </summary>
		Action<int, DateTime, TimeSpan, Raster.Image, Tuple<string, object>[]> Send { set; }
		/// <summary>
		/// Tries to open <paramref name="name"/> with player.
		/// </summary>
		/// <param name="name">URI specifying resource to open.</param>
		/// <returns>True if the resource was opened successfully.</returns>
		bool Open(Uri.Locator name);
		/// <summary>
		/// Closes the currently open resource.
		/// </summary>
		void Close();
		/// <summary>
		/// Status of the player which indicates if it has a resource open and whether it is playing it or not.
		/// </summary>
		Status Status { get; }
	}
}
