using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckLinkAPI;

namespace Imint.Media.Blackmagic
{
	public struct PixelFormat
	{
		public _BMDPixelFormat Format { get; private set; }

		public PixelFormat(Kean.Uri.Query query) :
			this()
		{
			this.Format = _BMDPixelFormat.bmdFormat8BitYUV;

			string colorspace = (query["format"] ?? "YUV").ToLower();
			List<_BMDPixelFormat> matches = new List<_BMDPixelFormat>();
			foreach (_BMDPixelFormat displayMode in Enum.GetValues(typeof(_BMDPixelFormat)))
				if (displayMode.ToString().ToLower().EndsWith(colorspace))
					matches.Add(displayMode);
			int depth = query.Get("depth", 8);
			foreach (_BMDPixelFormat match in matches)
			{
				string matchString = match.ToString();
				if (match.ToString().Contains(depth + "Bit"))
					this.Format = match;
			}
		}
	}
}
