using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckLinkAPI;
using Kean;
using Kean.Extension;

namespace Imint.Media.Blackmagic
{
	public struct DisplayMode
	{
		static float[] rates = { 23.98f, 24, 25, 29.97f, 30, 50, 59.94f, 60 };

		public _BMDDisplayMode Mode { get; private set; }

		public DisplayMode(Kean.Uri.Query query) :
			this()
		{
			this.Mode = _BMDDisplayMode.bmdModeUnknown;

			string resolution = (query["resolution"] ?? "unknown").ToLower();
			List<_BMDDisplayMode> matches = new List<_BMDDisplayMode>();
			foreach (_BMDDisplayMode displayMode in Enum.GetValues(typeof(_BMDDisplayMode)))
				if (displayMode.ToString().ToLower().Contains(resolution))
					matches.Add(displayMode);
			float rate = query.Get("rate", 0f);
			for (int i = 0; i < rates.Length; i++)
			{
				if (Math.Abs(rate - DisplayMode.rates[i]) < 0.01f)
				{
					rate = DisplayMode.rates[i];
					break;
				}
			}
			foreach(_BMDDisplayMode match in matches)
			{
				string matchString = match.ToString();
				string rateWithoutComma = rate.ToString("N2", System.Globalization.CultureInfo.InvariantCulture).TrimEnd('0').Replace(",", "").Replace(".", "").Replace(" ", "");
				if (matchString.EndsWith(rateWithoutComma) || matchString.EndsWith(rateWithoutComma + "00"))
				{
					this.Mode = match;
					break;
				}
			}
			if (this.Mode == _BMDDisplayMode.bmdModeUnknown)
			{
				switch (resolution)
				{
					case "ntsc" :
						this.Mode = _BMDDisplayMode.bmdModeNTSC;
						break;
					case "ntscp" :
						this.Mode = _BMDDisplayMode.bmdModeNTSCp;
						break;
					case "pal" :
						this.Mode = _BMDDisplayMode.bmdModePAL;
						break;
					case "palp" :
						this.Mode = _BMDDisplayMode.bmdModePALp;
						break;
					default :
						this.Mode = _BMDDisplayMode.bmdModeUnknown;
						break;
				}
			}
		}
	}
}
