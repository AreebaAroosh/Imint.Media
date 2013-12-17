using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Target = Imint.Media.Blackmagic.PixelFormat;
using DeckLinkAPI;
using Kean;
using Uri = Kean.Uri;

namespace Imint.Media.Blackmagic.Test
{
	class PixelFormat :
		Kean.Test.Fixture<PixelFormat>
	{
		protected override void Run()
		{
			this.Run(
				this.Equality
			);
		}
		[Test]
		public void Equality()
		{
			Blackmagic.PixelFormat SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=rgb&depth=10");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat10BitRGB));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=rgbx&depth=10");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat10BitRGBX));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=rgbxle&depth=10");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat10BitRGBXLE));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=yuv&depth=10");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat10BitYUV));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=argb&depth=8");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat8BitARGB));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=bgra&depth=8");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat8BitBGRA));
			SomePixelFormat = new Blackmagic.PixelFormat((Uri.Query)"format=yuv&depth=8");
			Verify(SomePixelFormat.Format, Is.EqualTo(_BMDPixelFormat.bmdFormat8BitYUV));
		}
	}
}
