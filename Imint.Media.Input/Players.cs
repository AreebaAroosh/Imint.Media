﻿// 
//  Players.cs
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
using Kean.Core.Extension;
using Kean.Core.Collection.Extension;
using Raster = Kean.Draw.Raster;
using Collection = Kean.Core.Collection;
using Error = Kean.Core.Error;
using Parallel = Kean.Core.Parallel;
using Uri = Kean.Core.Uri;
using Platform = Kean.Platform;
using Reflect = Kean.Core.Reflect;
using Kean.Core.Reflect.Extension;

namespace Imint.Media.Input
{
	public class Players :
		Collection.Wrap.List<Player.IStream>,
		IDisposable
	{
		internal Platform.Application Application
		{
			set
			{
				foreach (Player.IStream stream in this)
					if (stream is Platform.IHasApplication)
						(stream as Platform.IHasApplication).Application = value;
			}
		}
		object @lock;
		Collection.Hooked.List<Player.IStream> data;
		bool hasNext;
		bool isLinear;
		bool isNonLinear;
		Status status;
		DateTime? start;
		Player.IStream player;
		System.Threading.Thread sender;
		DateTime end;
		DateTime position;
		object incomingLock = new object();
		bool? incomingPlaying;
		DateTime? incomingSeek;
		public int Channels { get; private set; }
		Collection.List<string> supportedExtensions = new Collection.List<string>();
		public Collection.List<string> SupportedExtensions { get { return supportedExtensions; } }
		public Action<Frame> Send { get; set; }
		public Action<bool, bool, DateTime, DateTime, DateTime, bool, bool, bool> UpdateState { get; set; }
		public Players() :
			this(new Collection.Hooked.List<Player.IStream>(), new object())
		{ }
		Players(Collection.Hooked.List<Player.IStream> hooked, object @lock) :
			base(new Collection.Synchronized.List<Player.IStream>(hooked, @lock))
		{
			this.data = hooked;

			this.@lock = @lock;
			hooked.Added += (index, player) =>
			{
				if (player.NotNull())
					lock (player)
					{
						if (player.Channels > this.Channels)
							this.Channels = player.Channels;
						if (player is Player.ILinear && (player as Player.ILinear).IsLinear)
							player.Send = this.WrappedSend;
						else
							player.Send = (int channel, DateTime time, TimeSpan lifetime, Raster.Image content, Tuple<string, object>[] meta) =>
							{
								bool freezed;
								lock (this.@lock)
								{
									if (!(freezed = this.status != Status.Playing))
									{
										if (!this.start.HasValue)
											this.start = time;
										this.position = time;
									}
									this.end = time;
								}
								if (!freezed)
									this.WrappedSend(channel, time, lifetime, content, meta);
							};
						if (player is Player.IFile)
							(this.supportedExtensions as Collection.IList<string>).Add((player as Player.IFile).SupportedExtensions);
					}
			};
		}
		void WrappedSend(int channel, DateTime time, TimeSpan lifetime, Raster.Image content, Tuple<string, object>[] meta)
		{
			if (Error.Log.CatchErrors)
			{
				try
				{
					this.Send.Call(new Frame(channel, time, lifetime, content, meta));
				}
				catch (System.Exception e)
				{
					Error.Log.Append(Error.Level.Recoverable, "Input Failed to Send Frame.", e);
				}
			}
			else
				this.Send.Call(new Frame(channel, time, lifetime, content, meta));
		}
		public bool Open(Uri.Locator resource)
		{
			bool result = false;
			if (this.status == Status.Closed)
			{
				System.Threading.AutoResetEvent wait = new System.Threading.AutoResetEvent(false);
				Action<Uri.Locator> run = argument =>
				{
					Player.IStream player = null;
					Uri.Locator path = argument as Uri.Locator;
					foreach (Player.IStream p in this)
						if (p.NotNull() && p.Open(path))
						{
							Error.Log.Append(Error.Level.Message, "Open Succeeded", "Successfully opened \"" + path + "\" with player \"" + p.Type());
							lock (this.@lock)
							{
								player = p;
								this.player = player;
								this.status = this.player.Status;
								this.hasNext = path.Query["video"].NotEmpty() && p is Player.INonLinear;
								result = true;
							}
							break;
						}
					wait.Set();
					if (player.NotNull())
						try
						{
							lock (this.incomingLock)
							{ // throw away all old incoming commands
								this.incomingPlaying = null;
								this.incomingSeek = null;
							}
							while (player.Status != Status.Closed)
							{
								player.Poll();
								bool isLinear = false;
								bool isNonLinear = false;
								if (player is Player.ILinear && (player as Player.ILinear).IsLinear)
								{
									{
										bool? playing;
										DateTime? seek;
										lock (this.incomingLock)
										{
											playing = this.incomingPlaying;
											seek = this.incomingSeek;
											this.incomingPlaying = null;
											this.incomingSeek = null;
										}
										if (playing.HasValue)
										{
											if (playing.Value)
												(player as Player.ILinear).Play();
											else
												(player as Player.ILinear).Pause();
										}
										if (player is Player.INonLinear && (player as Player.INonLinear).IsNonLinear && seek.HasValue)
											(player as Player.INonLinear).Seek(seek.Value);
									}
									isLinear = player is Player.ILinear && (player as Player.ILinear).IsLinear;
									isNonLinear = player is Player.INonLinear && (player as Player.INonLinear).IsNonLinear;
									DateTime position = (player as Player.ILinear).Position;
									DateTime start = isNonLinear ? (player as Player.INonLinear).Start : new DateTime();
									DateTime end = isNonLinear ? (player as Player.INonLinear).End : position;

									lock (this.@lock)
									{
										this.start = start;
										this.end = end;
										this.position = position;
										this.isLinear = isLinear;
										this.isNonLinear = isNonLinear;
									}
								}
								else
								{
									bool freezed;
									lock (this.incomingLock)
										freezed = this.incomingPlaying.HasValue && !this.incomingPlaying.Value;
									lock (this.@lock)
										this.status = freezed ? Status.Paused : Status.Playing;
								}
								lock (this.@lock)
								{
									if (player is Player.ILinear && (player as Player.ILinear).IsLinear)
										this.status = player.Status;
									this.UpdateState(this.status == Status.Closed, this.status == Status.Playing, this.start ?? new DateTime(), this.end, this.position, isLinear, isNonLinear, this.hasNext);
								}
							}
						}
						finally
						{
							player.Close();
						}
				};
				Action<Uri.Locator> wrappedRun = Error.Log.CatchErrors ?
					argument =>
					{
						try
						{
							run(argument);
						}
						catch(System.Exception e)
						{
							Error.Log.Append(Error.Level.Recoverable, result ? "Failed During Playback of Media \"" + argument + "\"" : "Failed to Open Media \"" + argument + "\"", e);
							result = false;
							wait.Set();
						}
					} :
					run;
				this.sender = new System.Threading.Thread(new System.Threading.ParameterizedThreadStart(a => wrappedRun(a as Uri.Locator))) { Name = "Input Thread" };
				this.sender.Start(resource);
				wait.WaitOne();
			}
			return result;
		}
		public void Close()
		{
			if (this.sender.NotNull())
			{
				this.sender.Abort();
				this.sender.Join(1000);
				this.sender.Interrupt();
				this.sender = null;
			}
			this.status = Status.Closed;
			this.incomingLock = new object();
			this.incomingPlaying = null;
			this.incomingSeek = null;
		}
		public void Play()
		{
			lock (this.incomingLock)
				this.incomingPlaying = true;
		}
		public void Pause()
		{
			lock (this.incomingLock)
				this.incomingPlaying = false;
		}
		public void Seek(DateTime position)
		{
			lock (this.incomingLock)
				this.incomingSeek = position;
		}

		public void Dispose()
		{
			this.Close();
			if (this.data.NotNull())
				foreach (Player.IStream stream in this.data)
					if (stream.NotNull())
						stream.Dispose();
		}
	}
}
