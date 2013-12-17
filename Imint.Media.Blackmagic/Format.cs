using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckLinkAPI;

namespace Imint.Media.Blackmagic
{
	enum Format
	{
		frmt8BitARGB = _BMDPixelFormat.bmdFormat8BitARGB,
		frmt8BitYUV = _BMDPixelFormat.bmdFormat8BitYUV,
		frmt8BitBGRA = _BMDPixelFormat.bmdFormat8BitBGRA,
		frmt10BitRGBX = _BMDPixelFormat.bmdFormat10BitRGBX,
		frmt10BitRGBXLE = _BMDPixelFormat.bmdFormat10BitRGBXLE,
		frmt10BitRGB = _BMDPixelFormat.bmdFormat10BitRGB,
		frmt10BitYUV = _BMDPixelFormat.bmdFormat10BitYUV,
	}
}
