using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using NUnit.Framework;
using Target = Imint.Media.Blackmagic.Connection;
using DeckLinkAPI;
using Kean;
using Uri = Kean.Uri;

namespace Imint.Media.Blackmagic.Test
{
	class Connection :
		Kean.Test.Fixture<Connection>
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
			Blackmagic.Connection SomeConnection = new Blackmagic.Connection((Uri.Query)"input=Component", _BMDVideoConnection.bmdVideoConnectionSDI);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionComponent));
			SomeConnection = new Blackmagic.Connection((Uri.Query)"input=Composite", _BMDVideoConnection.bmdVideoConnectionSDI);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionComposite));
			SomeConnection = new Blackmagic.Connection((Uri.Query)"input=HDMI", _BMDVideoConnection.bmdVideoConnectionSDI);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionHDMI));
			SomeConnection = new Blackmagic.Connection((Uri.Query)"input=OpticalSDI", _BMDVideoConnection.bmdVideoConnectionSDI);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionOpticalSDI));
			SomeConnection = new Blackmagic.Connection((Uri.Query)"input=SDI", _BMDVideoConnection.bmdVideoConnectionSVideo);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionSDI));
			SomeConnection = new Blackmagic.Connection((Uri.Query)"input=SVideo", _BMDVideoConnection.bmdVideoConnectionSDI);
			Verify(SomeConnection.Cable, Is.EqualTo(_BMDVideoConnection.bmdVideoConnectionSVideo));
		}
	}
}
