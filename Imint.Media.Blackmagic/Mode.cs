using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DeckLinkAPI;

namespace Imint.Media.Blackmagic
{
	enum Mode
	{
		dspmdHD1080p2398 = _BMDDisplayMode.bmdModeHD1080p2398,
		dspmdHD1080p24 = _BMDDisplayMode.bmdModeHD1080p24,
		dspmd2k2398 = _BMDDisplayMode.bmdMode2k2398,
		dspmd2k24 = _BMDDisplayMode.bmdMode2k24,
		dspmd2k25 = _BMDDisplayMode.bmdMode2k25,
		dspmd4kDCI2398 = _BMDDisplayMode.bmdMode4kDCI2398,
		dspmd4kDCI24 = _BMDDisplayMode.bmdMode4kDCI24,
		dspmd4kDCI25 = _BMDDisplayMode.bmdMode4kDCI25,
		dspmd4K2160p2398 = _BMDDisplayMode.bmdMode4K2160p2398,
		dspmd4K2160p24 = _BMDDisplayMode.bmdMode4K2160p24,
		dspmd4K2160p25 = _BMDDisplayMode.bmdMode4K2160p25,
		dspmd4K2160p2997 = _BMDDisplayMode.bmdMode4K2160p2997,
		dspmd4K2160p30 = _BMDDisplayMode.bmdMode4K2160p30,
		dspmdHD1080i50 = _BMDDisplayMode.bmdModeHD1080i50,
		dspmdHD1080i5994 = _BMDDisplayMode.bmdModeHD1080i5994,
		dspmdHD1080i60M = _BMDDisplayMode.bmdModeHD1080i6000,
		dspmdHD1080p25 = _BMDDisplayMode.bmdModeHD1080p25,
		dspmdHD1080p2997 = _BMDDisplayMode.bmdModeHD1080p2997,
		dspmdHD1080p30 = _BMDDisplayMode.bmdModeHD1080p30,
		dspmdHD1080p50 = _BMDDisplayMode.bmdModeHD1080p50,
		dspmdHD1080p5994 = _BMDDisplayMode.bmdModeHD1080p5994,
		dspmdHD1080p60 = _BMDDisplayMode.bmdModeHD1080p6000,
		dspmdHD720p50 = _BMDDisplayMode.bmdModeHD720p50,
		dspmdHD720p5994 = _BMDDisplayMode.bmdModeHD720p5994,
		dspmdHD720p60 = _BMDDisplayMode.bmdModeHD720p60,
		dspmdUnknown = _BMDDisplayMode.bmdModeUnknown,
		dspmdNTSC2398 = _BMDDisplayMode.bmdModeNTSC2398,
		dspmdNTSC = _BMDDisplayMode.bmdModeNTSC,
		dspmdNTSCp = _BMDDisplayMode.bmdModeNTSCp,
		dspmdPAL = _BMDDisplayMode.bmdModePAL,
		dspmdPALp = _BMDDisplayMode.bmdModePALp,
	}
}
