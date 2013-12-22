using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Target = Imint.Media.Blackmagic.DisplayMode;
using DeckLinkAPI;
using Kean;
using Uri = Kean.Uri;

namespace Imint.Media.Blackmagic.Test
{
	class DisplayMode :
		Kean.Test.Fixture<DisplayMode>
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
			Blackmagic.DisplayMode SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=23.98");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p2398));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=24");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p24));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080&rate=24");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p24));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=2k&rate=23.98");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode2k2398));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=2k&rate=24");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode2k24));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=2k&rate=25");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode2k25));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4kDCI&rate=23.98");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4kDCI2398));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4kDCI&rate=24");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4kDCI24));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4kDCI&rate=25");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4kDCI25));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4K2160p&rate=23.98");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4K2160p2398));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4K2160P&rate=24");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4K2160p24));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4K2160p&rate=25");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4K2160p25));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4K2160p&rate=29.97");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4K2160p2997));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=4K2160p&rate=30");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdMode4K2160p30));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080i&rate=50");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080i50));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080i&rate=59.94");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080i5994));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080i&rate=60.00");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080i6000));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080&rate=60.00");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080i6000));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=25");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p25));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=29.97");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p2997));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=30");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p30));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=50");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p50));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=59.94");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p5994));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD1080p&rate=60.00");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD1080p6000));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD720p&rate=50");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD720p50));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD720p&rate=59.94");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD720p5994));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=HD720p&rate=60");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeHD720p60));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=Unknown");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeUnknown));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=NTSC&rate=23.98");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeNTSC2398));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=NTSC");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeNTSC));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=NTSCp");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModeNTSCp));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=PAL");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModePAL));
			SomeMode = new Blackmagic.DisplayMode((Uri.Query)"resolution=PALp");
			Verify(SomeMode.Mode, Is.EqualTo(_BMDDisplayMode.bmdModePALp));
		}
	}
}
