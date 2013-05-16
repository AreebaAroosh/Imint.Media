// 
//  Stream.cs
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
using Kean.Core;
using Kean.Core.Extension;
using Bitmap = Kean.Draw.Raster;
using Uri = Kean.Core.Uri;
using Log = Kean.Platform.Log;
using Error = Kean.Core.Error;
using Serialize = Kean.Core.Serialize;
using Platform = Kean.Platform;

namespace Imint.Media.DirectShow
{
	public abstract class Stream :
		Media.Player.IStream,
		Platform.IHasApplication,
		IDisposable
	{
		Action<int, DateTime, TimeSpan, Bitmap.Image, Tuple<string, object>[]> sendFrame;
		protected DirectShow.Binding.IGraph Graph { get; set; }
		protected Kean.Math.Fraction Rate { get; private set; }

		[Serialize.Parameter]
		public bool Fuzzy { get; set; }
		[Serialize.Parameter]
		public bool Debug { get; set; }
		protected Stream()
		{
		}
		~Stream()
		{
			Error.Log.Wrap((Action)this.Dispose)();
		}
		#region IDisposable Members
		public void Dispose()
		{
			(this as Media.Player.IStream).Close();
            if (this.sendFrame.IsNull())
                this.sendFrame = null;
		}
		#endregion

		protected abstract DirectShow.Binding.IGraph Open(Uri.Locator name);
		protected virtual void ParseArguments(Uri.Locator name)
		{
			this.Rate = name.Query["rate"];
		}
		#region IStream Members
		int Media.Player.IStream.Channels { get { return 1; } }
		void Media.Player.IStream.Poll() { System.Threading.Thread.Sleep(10); }
		Action<int, DateTime, TimeSpan, Bitmap.Image, Tuple<string, object>[]> Media.Player.IStream.Send
		{
			set { this.sendFrame = value; }
		}
		bool Media.Player.IStream.Open(Uri.Locator name)
		{
			this.ParseArguments(name);
			bool result = (this.Graph = this.Open(name)).NotNull();
			if (result)
			{
				if (this.Debug)
					this.Graph.Save(Uri.Locator.FromPlatformPath(Environment.SpecialFolder.MyDocuments, "graph.grf"));
				this.Graph.Send = (DateTime position, TimeSpan lifeTime, Bitmap.Image frame) => 
				{
					if (this.Graph.NotNull()) 
						this.sendFrame(0, position, lifeTime, frame, null); 
				};
			}
			return result;
		}
		void Media.Player.IStream.Close()
		{
			if (this.Graph.NotNull())
			{
				this.Graph.Close();
				this.Graph = null;
			}
		}
		Status Media.Player.IStream.Status
		{
			get { return this.Graph.NotNull() ? this.Graph.Status : Status.Closed; }
		}
		#endregion

		#region IHasApplication Members
		public Platform.Application Application { get; set; }
		#endregion
	}
}
