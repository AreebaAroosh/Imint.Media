using System;
using Kean;
using Kean.Extension;
using Uri = Kean.Uri;
using IO = Kean.IO;


namespace Imint.Media.MotionJpeg.Http
{
	class Part :
		IO.IByteInDevice
	{
		IO.IByteInDevice backend;
		byte[] endMark;
		int matchingLength;
		int nextPosition;
		public event Action Closed;
		byte? peeked;
		public bool Empty { get { return this.backend.IsNull() || this.backend.Empty; } }
		public Uri.Locator Resource { get { return this.backend.NotNull() ? this.backend.Resource : null; } }
		public bool Opened { get { return this.backend.NotNull() && this.backend.Opened; } }
		public Part(IO.IByteInDevice backend, byte[] endMark)
		{
			this.backend = backend;
			this.endMark = endMark;
		}
		long length;
		byte? Next()
		{
			byte? result;
			if (this.nextPosition < this.matchingLength)
				result = this.endMark[this.nextPosition++];
			else if (this.backend.IsNull())
				result = null;
			else
			{
				byte? next = result = this.backend.Read();
				if (next.HasValue && next.Value == this.endMark[0])
				{
					this.nextPosition = 1;
					this.matchingLength = 1;
					while (true)
					{
						if (this.matchingLength < this.endMark.Length)
						{
							while (!(next = this.backend.Peek()).HasValue)
								;
							if (next.Value != this.endMark[this.matchingLength])
							{
								break;
							}
							this.matchingLength++;
							next = this.backend.Read();
						}
						else
						{
							this.matchingLength = 0;
							this.backend = null;
							result = null;
							break;
						}
					}
				}
			}
			if (result.HasValue)
				this.length++;
			return result;
		}
		public byte? Peek()
		{
			return this.peeked.HasValue ? this.peeked : this.peeked = this.Next();
		}
		public byte? Read()
		{
			byte? result;
			if (this.peeked.HasValue)
			{
				result = this.peeked;
				this.peeked = null;
			}
			else
				result = this.Next();
			return result;
		}
		public bool Close()
		{
			bool result;
			this.backend = null;
			if (result = this.Closed.NotNull())
			{
				Console.WriteLine("Length: " + this.length);
				this.Closed();
				this.Closed = null;
			}
			return result;
		}
		void IDisposable.Dispose()
		{
			this.Close();
		}
	}
}
