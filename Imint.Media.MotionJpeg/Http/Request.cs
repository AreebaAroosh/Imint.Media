using System;
using Kean;
using Kean.Extension;
using Uri = Kean.Uri;

namespace Imint.Media.MotionJpeg.Http
{
	public class Request
	{
		public Uri.Locator Url { get; set; }
		public Request()
		{
		}
		public Response Connect()
		{
			return Response.Open(this);
		}
	}
}
