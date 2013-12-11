using System;
using Kean;
using Kean.Extension;
using Uri = Kean.Uri;
using IO = Kean.IO;
using Kean.IO.Extension;
using Collection = Kean.Collection;
using Kean.Collection.Extension;
using Generic = System.Collections.Generic;

namespace Imint.Media.MotionJpeg.Http
{
	public class Response : 
		IDisposable
	{
		System.Net.HttpWebRequest request;
		System.Net.WebResponse response;
		public string ContentType { get { return this.response.ContentType; } }
		internal Response(System.Net.HttpWebRequest request, System.Net.WebResponse response)
		{
			this.request = request;
			this.response = response;
		}
		~Response()
		{
			this.Close();
		}
		public IO.IByteInDevice Open()
		{
			return IO.ByteDevice.Open(this.response.GetResponseStream());
		}
		public bool Open(Func<string, IO.IByteInDevice, bool> process)
		{
			return this.Process(this.ContentType, this.Open(), process);
		}
		bool Process(string contentType, IO.IByteInDevice device, Func<string, IO.IByteInDevice, bool> process)
		{
			bool result = false;
			if (contentType.NotNull() && contentType.StartsWith("multipart/x-mixed-replace"))
			{
				byte[] boundary = this.GetBoundary(contentType);
				device.MovePast(boundary);
				while (this.response.NotNull() && device.Opened)
				{
					Collection.IDictionary<string, string> headers = new Collection.Dictionary<string, string>(
						device.ReadPast(13, 10, 13, 10).Decode().Join()
						.Split(new string[] { "\r\n" }, StringSplitOptions.RemoveEmptyEntries)
						.Map(header => { string[] splitted = header.Split(new string[] { ": " }, 2, StringSplitOptions.None); return splitted.Length == 2 ? KeyValue.Create(splitted[0], splitted[1]) : KeyValue.Create((string)null, (string)null); }));
					this.Process(headers["Content-Type"], new Part(device, boundary), process);
				}
			}
			else
				result = process(contentType, device);
			return result;
		}
		byte[] GetBoundary(string contentType)
		{
			byte[] result = null;
			string extractBoundary = contentType.Split(new char[] { ';' }, StringSplitOptions.RemoveEmptyEntries).Find(s => s.Contains("boundary"));
			string[] keyValue = extractBoundary.Split(new char[] { ' ', '=' }, StringSplitOptions.RemoveEmptyEntries);
			if (keyValue.Length == 2 && keyValue[0] == "boundary")
				result = ("\r\n" + (keyValue[1].StartsWith("--") ? "" : "--") + keyValue[1] + "\r\n").AsBinary();
			return result;
		}
		public bool Close()
		{
			bool result;
			if (result = this.request.NotNull())
			{
				this.request.Abort();
				this.request = null;
			}
			if (result |= this.response.NotNull())
			{
				this.response.Close();
				this.response = null;
			}
			return result;
		}
		void IDisposable.Dispose()
		{
			this.Close();
		}
		public static Response Open(Request request)
		{
			Response result = null;
			System.Net.HttpWebRequest backendRequest = System.Net.WebRequest.Create(request.Url) as System.Net.HttpWebRequest;
			if (backendRequest.NotNull())
			{
				backendRequest.Credentials = request.Url.Authority.User.NotNull() && request.Url.Authority.User.Name.NotEmpty() && request.Url.Authority.User.Password.NotEmpty() ? new System.Net.NetworkCredential(request.Url.Authority.User.Name, request.Url.Authority.User.Password) : null;
				System.Net.WebResponse backendResponse = backendRequest.GetResponse();
				if (backendResponse.NotNull())
					result = new Response(backendRequest, backendResponse);
			}
			return result;
		}
	}
}
