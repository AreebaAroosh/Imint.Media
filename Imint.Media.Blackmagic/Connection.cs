using DeckLinkAPI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imint.Media.Blackmagic
{
	public struct Connection
	{
		public _BMDVideoConnection Cable { get; private set; }

		public Connection(Kean.Uri.Query query, _BMDVideoConnection oldConnection) :
			this()
		{
			this.Cable = oldConnection;
			string cable = (query["connection"] ?? "SDI").ToLower();
			if (cable == "sdi")
				this.Cable = _BMDVideoConnection.bmdVideoConnectionSDI;
			else if (cable == "opticalsdi")
				this.Cable = _BMDVideoConnection.bmdVideoConnectionOpticalSDI;
			else
				foreach (_BMDVideoConnection connection in Enum.GetValues(typeof(_BMDVideoConnection)))
					if (connection.ToString().ToLower().EndsWith(cable) && !connection.ToString().ToLower().EndsWith("SDI"))
						this.Cable = connection;
		}
	}
}
