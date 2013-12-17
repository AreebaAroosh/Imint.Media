using DeckLinkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imint.Media.Blackmagic
{
	enum Connection
	{
		SDI = _BMDVideoConnection.bmdVideoConnectionSDI,
		HDMI = _BMDVideoConnection.bmdVideoConnectionHDMI,
		OpticalSDI = _BMDVideoConnection.bmdVideoConnectionOpticalSDI,
		Component = _BMDVideoConnection.bmdVideoConnectionComponent,
		Composite =_BMDVideoConnection.bmdVideoConnectionComposite,
		SVideo = _BMDVideoConnection.bmdVideoConnectionSVideo,
	}
}
