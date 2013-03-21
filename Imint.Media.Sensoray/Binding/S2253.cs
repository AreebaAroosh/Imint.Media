// Copyright Sensoray Company Inc. 2012
namespace Sensoray
{
	using System;
	using System.Runtime.InteropServices;
	using System.ComponentModel;

	sealed class S2253
	{
		public enum MID2253_VIDSYS
		{
			MID2253_VIDSYS_PAL = 1,
			MID2253_VIDSYS_NTSC = 2
		};

		public enum MID2253_STREAMTYPE
		{
			MID2253_STYPE_MPEG4 = 0,
			MID2253_STYPE_H264 = 1,
			MID2253_STYPE_MJPEG = 2,
			MID2253_STYPE_UYVY = 3,
			MID2253_STYPE_Y800 = 4,
			MID2253_STYPE_M4TS = 5, /* MPEG4 in MPEG transport stream */
			MID2253_STYPE_H4TS = 6, /* H264 in MPEG transport stream */

		};

		public enum MID2253_OSDTYPE
		{
			MID2253_OSDTYPE_TEXT = 1,
			MID2253_OSDTYPE_BMP = 2 // future use
		};

		public enum MID2253_AUDIO_INPUT
		{
			MID2253_AUDIO_MIC = 0,
			MID2253_AUDIO_LINE = 1
		};

		public enum MID2253_OUTPUT_MODE
		{
			MID2253_OUTPUT_IDLE = 0,
			MID2253_OUTPUT_PASSTHRU = 1,
			MID2253_OUTPUT_COLORBARS = 2,
			MID2253_OUTPUT_FLASH = 3,
			MID2253_OUTPUT_STREAM = 4
		};

		public enum MID2253_PARAM
		{
			MID2253_PARAM_FIRMWARE = 0
		};

		public enum MID2253_RECMODE
		{
			MID2253_RECMODE_VIDEO = 0,    // only record video (muxed: .MP4 for MPEG4/H264, .AVI for uncompressed)
			MID2253_RECMODE_AV = 1,       // audio plus video (.MP4 for MPEG4/H264, .AVI for uncompressed)
			MID2253_RECMODE_VES = 2       // only record video elementary stream (AVI mux is used for uncompressed)
		};

		public enum MID2253_MP4MODE
		{
			MID2253_MP4MODE_STANDARD = 0,  // same as MID2253_RECMODE_VIDEO.
			MID2253_MP4MODE_STREAMABLE = 1 // streamable fragmented MP4 (May not play on standard players).
		};

		public enum MID2253_AUDENC
		{
			MID2253_AUDENC_PCM = 0,          // "no" audio encoding.
			MID2253_AUDENC_G711_ULAW = 2,
			MID2253_AUDENC_G711_ALAW = 3
		};

		public enum MID2253_AUDIO_BITRATE
		{
			MID2253_AUDBR_96 = 96,
			MID2253_AUDBR_128 = 128,
			MID2253_AUDBR_192 = 192,
			MID2253_AUDBR_224 = 224,
			MID2253_AUDBR_256 = 256,
		};

		public enum MID2253_GPIO_SIGNAL
		{
			MID2253_GPIO_LOW,
			MID2253_GPIO_HIGH,
			MID2253_GPIO_CHANGE
		};
		public enum MID2253_PREVIEWTYPE
		{
			MID2253_PREVIEWTYPE_DEFAULT = 0, // DLL chooses best available setting. 
			MID2253_PREVIEWTYPE_VMR9 = 1,    // Force use of VMR 9.
			MID2253_PREVIEWTYPE_VMR7 = 2,    // Force use of VMR 7
			MID2253_PREVIEWTYPE_LEGACY = 3,  //  legacy renderer only supported as pop-up window.
			//XP and later versions of Windows support at least VMR7.
		};
		// Constants
		public const int MID2253_LEVEL_CONTRAST = 1;
		public const int MID2253_LEVEL_BRIGHTNESS = 2;
		public const int MID2253_LEVEL_SATURATION = 4;
		public const int MID2253_LEVEL_HUE = 8;

		public const int MID2253_OVERLAY_UPDATE_DISPLAY = 1;
		public const int MID2253_OVERLAY_UPDATE_POSITION = 2;
		public const int MID2253_OVERLAY_UPDATE_TRANSPARENT = 4;

		public const int MUX_ITEMDATA_MP4 = 0;
		public const int MUX_ITEMDATA_VES = 1;
		public const int MUX_ITEMDATA_MP4F = 2;
		public struct RECTANGLE
		{
			public Int32 left;
			public Int32 top;
			public Int32 right;
			public Int32 bottom;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct MID2226STATUS
		{
			public UInt32 iFileSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szFilePath;
			public Boolean bIsRecording;
			public Boolean bIsPlaying;
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct MID2226STATUS_W
		{
			UInt32 iFileSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			String szFilePath;
			Boolean bIsRecording;
			Boolean bIsPlaying;
		};

		[StructLayout(LayoutKind.Sequential)]
		public struct MID2253STATUS
		{
			public UInt32 iFileSize;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szFilePath;
			public Boolean bIsRecording;
			public Boolean bIsRunning;
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 260)]
			public String szClipPath;
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct MID2253OSD
		{
			public Int32 osdOn;    //      // toggles osd on or off.
			public Int32 osdChan;  // osd channel to update.  osdChan=0 for stream A, osdChan=1 for stream B
			public Int32 transparent;  //whether text transparent or not
			public Int32 positionTop;  //see xoff, yoff below
			public Int32 ddmm;     // ddmm=1 puts the day before the month
			public Int32 year2;    // year display mode (year2 = 1 means 2 digits, year2=0 means 4 digits)
			public Int32 fraction;  // whether to display fraction of seconds
			[MarshalAs(UnmanagedType.ByValTStr, SizeConst = 80)]
			public String line;     // ascii string of text (null terminated)
			public Int32 xoffset;  //x offset: if positionTop=1, relative to top. if positionTop=0, relative to bottom
			public Int32 yoffset;  //y offset: if positionTop=1, relative to top. if positionTop=0, relative to bottom
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct MID2253CLOCK
		{
			public UInt64 sec;
			public UInt32 usec;
		};
		[StructLayout(LayoutKind.Sequential)]
		public struct MID2253_OVL_STRUCT
		{
			public Byte id;
			public Byte transparent;
			public Byte update;
			public Byte reserved;
			public Int32 xOffset;
			public Int32 yOffset;
			public UInt32 length;
		};

		public static GCHandle gch;

		public static Int32 OpenBoard(Int32 idx)
		{
			return S2253_Open(idx);
		}
		public static Int32 CloseBoard(Int32 hdev)
		{
			return S2253_Close(hdev);
		}
		public static Int32 SetBitrate(Int32 bitrate, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetBitrate(bitrate, devid, strmidx);
			return hr;
		}
		public static Int32 SetVidSys(MID2253_VIDSYS vsys, Int32 devid)
		{
			int hr;
			hr = S2253_SetVidSys(vsys, devid);
			return hr;
		}
		public static Int32 GetVidSys(ref MID2253_VIDSYS vsys, Int32 devid)
		{
			int hr;
			hr = S2253_GetVidSys(ref vsys, devid);
			return hr;
		}

		public static Int32 SetStreamType(Int32 strmtype, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetStreamType(strmtype, devid, strmidx);
			return hr;
		}

		public static Int32 GetStreamType(ref Int32 strmtype, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetStreamType(ref strmtype, devid, strmidx);
			return hr;
		}

		public static Int32 SetClock(ref MID2253CLOCK clk, Int32 devid)
		{
			int hr;
			hr = S2253_SetClock(ref clk, devid);
			return hr;

		}
		public static Int32 SetRecordMode(MID2253_RECMODE recmode, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetRecordMode(recmode, devid, strmidx);
			return hr;
		}

		public static Int32 GetRecordMode(ref MID2253_RECMODE recmode, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetRecordMode(ref recmode, devid, strmidx);
			return hr;
		}
		public static Int32 SetMp4Mode(MID2253_MP4MODE mode, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetMp4Mode(mode, devid, strmidx);
			return hr;
		}

		public static Int32 GetRecordMode(ref MID2253_MP4MODE mode, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetMp4Mode(ref mode, devid, strmidx);
			return hr;
		}
		public static Int32 SetJpegQ(Int32 q, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetJpegQ(q, devid, strmidx);
			return hr;
		}

		public static Int32 GetRecordMode(ref Int32 q, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetJpegQ(ref q, devid, strmidx);
			return hr;
		}

		public static Int32 GetStatus(ref MID2253STATUS status, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetStatus(ref status, devid, strmidx);
			return hr;
		}
		public static Int32 StartRecord([MarshalAs(UnmanagedType.LPWStr)] String filename, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_StartRecord(filename, 0, devid, strmidx);
			return hr;
		}
		public static Int32 SetLevel(Int32 param, Int32 level, Int32 devid)
		{
			int hr;
			Byte bval;
			bval = (Byte)level;
			hr = S2253_SetLevel(param, bval, devid);
			return hr;
		}
		public static Int32 GetLevel(Int32 param, ref Int32 level, Int32 devid)
		{
			int hr;
			Byte bval = 0;
			hr = S2253_GetLevel(param, ref bval, devid);
			level = bval;
			return hr;
		}

		public static Int32 SetAudioGain(Boolean bAGC, Int32 gain, Int32 devid)
		{
			int hr;
			hr = S2253_SetAudioGain(bAGC, gain, devid);
			return hr;
		}

		public static Int32 GetAudioGain(ref Boolean bAGC, ref Int32 gain, Int32 devid)
		{
			int hr;
			hr = S2253_GetAudioGain(ref bAGC, ref gain, devid);
			return hr;
		}


		public static Int32 StartCallback(Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_StartCallback(devid, strmidx);
			return hr;
		}
		public static Int32 RegisterCallback(DataCallback cb, Int32 devid, Int32 strmidx)
		{
			int hr = 0;
			gch = GCHandle.Alloc(cb);
			hr = S2253_RegisterCallback(cb, devid, strmidx);
			return hr;
		}
		public static Int32 StartDecode(String filename, int bUnicode, Int32 devid)
		{
			int hr;
			hr = S2253_StartDecode(filename, bUnicode, devid);
			return hr;
		}
		public static Int32 SetPreviewType(MID2253_PREVIEWTYPE type, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetPreviewType(type, devid, strmidx);
			return hr;
		}
		public static Int32 StartPreview(Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_StartPreview(devid, strmidx);
			return hr;
		}

		public static Int32 StopStream(Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_StopStream(devid, strmidx);
			if (gch.IsAllocated)
				gch.Free();
			return hr;
		}

		public static Int32 SetOsd(MID2253_OSDTYPE osdtype, ref MID2253OSD osd, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetOsd(osdtype, ref osd, devid, strmidx);
			return hr;
		}
		public static Int32 SetNotify(IntPtr handle, UInt32 mMessage, Int32 devid)
		{
			int hr;
			hr = S2253_SetNotify(handle, mMessage, devid);
			return hr;
		}
		public static Int32 TestDeviceRemoval(Int32 devid)
		{
			int hr;
			hr = S2253_TestDeviceRemoval(devid);
			return hr;
		}
		public static Int32 TestDecodeDone(Int32 devid)
		{
			int hr;
			hr = S2253_TestDecodeDone(devid);
			return hr;
		}
		public static Int32 RepaintWindow(IntPtr hdc, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_RepaintWindow(hdc, devid, strmidx);
			return hr;
		}
		public static Int32 GetNumDevices(ref Int32 pNumDevices)
		{
			int hr;
			hr = S2253_GetNumDevices(ref pNumDevices);
			return hr;
		}
		public static Int32 GetSerialNumber(ref UInt32 serial_number, Int32 devid)
		{
			int hr;
			hr = S2253_GetSerialNumber(ref serial_number, devid);
			return hr;
		}

		public static Int32 GetParam(MID2253_PARAM type, ref Int32 val, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetParam(type, ref val, devid, strmidx);
			return hr;
		}
		public static Int32 StartAudioPreview(Int32 devid)
		{
			int hr;
			hr = S2253_StartAudioPreview(devid);
			return hr;
		}
		public static Int32 StopAudioPreview(Int32 devid)
		{
			int hr;
			hr = S2253_StopAudioPreview(devid);
			return hr;
		}
		public static Int32 SetStreamWindow(IntPtr hwnd, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetStreamWindow(hwnd, devid, strmidx);
			return hr;
		}
		public static Int32 SetStreamWindowPosition(ref RECTANGLE rect, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetStreamWindowPosition(ref rect, devid, strmidx);
			return hr;
		}
		public static Int32 SetInterpolateMode(Int32 val, Int32 devid)
		{
			int hr;
			hr = S2253_SetInterpolateMode(val, devid);
			return hr;
		}
		public static Int32 GetInterpolateMode(ref Int32 val, Int32 devid)
		{
			int hr;
			hr = S2253_GetInterpolateMode(ref val, devid);
			return hr;
		}

		public static Int32 FlipImage(Boolean bFlipV, Boolean bFlipH, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_FlipImage(bFlipV, bFlipH, devid, strmidx);
			return hr;
		}
		public static Int32 GetGpioInput(ref Int32 value, Int32 devid)
		{
			int hr;
			hr = S2253_GetGpioInput(ref value, devid);
			return hr;
		}
		public static Int32 SetGpioOutput(Int32 value, Int32 devid)
		{
			int hr;
			hr = S2253_SetGpioOutput(value, devid);
			return hr;
		}
		public static Int32 GetGpioOutput(ref Int32 value, Int32 devid)
		{
			int hr;
			hr = S2253_GetGpioOutput(ref value, devid);
			return hr;
		}
		public static Int32 SetAudioEncoding(MID2253_AUDENC aenc, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetAudioEncoding(aenc, devid, strmidx);
			return hr;
		}
		public static Int32 GetAudioEncoding(ref MID2253_AUDENC aenc, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetAudioEncoding(ref aenc, devid, strmidx);
			return hr;
		}
		public static Int32 SetAudioBitrate(MID2253_AUDIO_BITRATE br, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetAudioBitrate(br, devid, strmidx);
			return hr;
		}
		public static Int32 GetAudioBitrate(ref MID2253_AUDIO_BITRATE br, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetAudioBitrate(ref br, devid, strmidx);
			return hr;
		}
		public static Int32 SetImageSize(Int32 w, Int32 h, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetImageSize(w, h, devid, strmidx);
			return hr;
		}
		public static Int32 GetImageSize(ref Int32 w, ref Int32 h, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetImageSize(ref w, ref h, devid, strmidx);
			return hr;
		}
		public static Int32 GetSample(IntPtr data, UInt32 inlen, ref UInt32 outlen, Int32 to, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_GetSample(data, inlen, ref outlen, to, devid, strmidx);
			return hr;
		}
		public static Int32 EnableSnapshot(Int32 bOn, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_EnableSnapshot(bOn, devid, strmidx);
			return hr;
		}
		public static Int32 WaitGpioInput(MID2253_GPIO_SIGNAL signal, Int32 timout, Int32 devid)
		{
			int hr;
			hr = S2253_WaitGpioInput(signal, timout, devid);
			return hr;
		}

		public static Int32 SetAudioInput(MID2253_AUDIO_INPUT input, Int32 devid)
		{
			int hr;
			hr = S2253_SetAudioInput(input, devid);
			return hr;
		}

		public static Int32 SetIDR(Int32 val, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetIDR(val, devid, strmidx);
			return hr;
		}
		public static Int32 LowLatencyPreview(Boolean bON, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_LowLatencyPreview(bON, devid, strmidx);
			return hr;
		}
		public static Int32 StopDecode(Int32 devid)
		{
			int hr;
			hr = S2253_StopDecode(devid);
			return hr;
		}

		public static Int32 SetOverlay(ref MID2253_OVL_STRUCT ovl, IntPtr data, Int32 devid)
		{
			int hr;
			hr = S2253_SetOverlay(ref ovl, data, devid);
			return hr;
		}

		public static Int32 SetOutputMode(MID2253_OUTPUT_MODE mode, Int32 devid)
		{
			int hr;
			hr = S2253_SetOutputMode(mode, devid);
			return hr;
		}
		public static Int32 SetUserData(String data, Int32 len, Int32 interval, Int32 devid, Int32 strmidx)
		{
			int hr;
			hr = S2253_SetUserData(data, len, interval, devid, strmidx);
			return hr;
		}

		public static Int32 GetHVLock(ref Int32 hvlock, Int32 devid)
		{
			int hr;
			hr = S2253_GetHVLock(ref hvlock, devid);
			return hr;
		}

#if _WIN64
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_Open(Int32 brdidx);

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_Close(Int32 brdidx);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetNumDevices(ref Int32 pNumDevices);
    
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetVidSys(MID2253_VIDSYS vidsys, Int32 devid);

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetVidSys(ref MID2253_VIDSYS vidsys, Int32 devid);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetStatus(ref MID2253STATUS pStatus, Int32 devid, Int32 strmidx);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetRecordMode(MID2253_RECMODE recmode, Int32 devid, Int32 strmidx);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetRecordMode(ref MID2253_RECMODE recmode, Int32 devid, Int32 strmidx);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetMp4Mode(MID2253_MP4MODE mp4mode, Int32 devid, Int32 strmidx);
    

        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetMp4Mode(ref MID2253_MP4MODE mp4mode, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetAudioEncoding(MID2253_AUDENC aenc, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetAudioEncoding(ref MID2253_AUDENC aenc, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetAudioBitrate(MID2253_AUDIO_BITRATE audbr, Int32 devid, Int32 strmidx);
     
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetAudioBitrate(ref MID2253_AUDIO_BITRATE audbr, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StartRecord(String filename, Int32 bUnicode, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetLevel(Int32 param, Byte value, Int32 devid);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetLevel(Int32 param, ref Byte value, Int32 devid);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StartPreview(Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StartAudioPreview(Int32 devid);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StopAudioPreview(Int32 devid);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StopStream(Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetImageSize(Int32 w, Int32 h, Int32 devid, Int32 strmidx);
    
        [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetImageSize(ref Int32 w, ref Int32 h, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetStreamType(Int32 stype, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetStreamType(ref Int32 stype, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_SetOsd(MID2253_OSDTYPE osdtype, ref MID2253OSD osd, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetClock(ref MID2253CLOCK clk, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetBitrate(Int32 bitrate, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetBitrate(ref Int32 bitrate, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetJpegQ(Int32 q, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetJpegQ(ref Int32 q, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetInterpolateMode(Int32 val, Int32 devid);
    
    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetInterpolateMode(ref Int32 val, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_StartSnapshot(Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetSample(IntPtr data, UInt32 inlen, ref UInt32 outlen, Int32 to, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_EnableSnapshot(Int32 bOn, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetSerialNumber(ref UInt32 serial_number, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_SetStreamWindow(IntPtr hwnd, Int32 devid, Int32 strmidx);
    
    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_SetStreamWindowPosition(ref RECTANGLE rect, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_RepaintWindow(IntPtr hdc, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetGpioInput(ref Int32 value, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetGpioOutput(Int32 value, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetGpioOutput(ref Int32 value, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_WaitGpioInput(MID2253_GPIO_SIGNAL signal, Int32 timout, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetAudioInput(MID2253_AUDIO_INPUT input, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetAudioGain(Boolean bAGC,  Int32 gain, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetAudioGain(ref Boolean bAGC, ref Int32 gain, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_FlipImage(Boolean bFlipV, Boolean bFlipH, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetIDR(Int32 val, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_LowLatencyPreview(Boolean bON, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_StartDecode(String filename, Int32 bUnicode, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_StopDecode(Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_SetNotify(IntPtr hNotifyAPp, UInt32 mNotifyMsg, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_TestDeviceRemoval(Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_TestDecodeDone(Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
       static extern Int32 S2253_SetOverlay(ref MID2253_OVL_STRUCT ovl, IntPtr data, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetOutputMode(MID2253_OUTPUT_MODE mode, Int32 devid);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_GetParam(MID2253_PARAM type, ref Int32 val, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_SetUserData(String data, Int32 len, Int32 interval, Int32 devid, Int32 strmidx);
    

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_StartCallback(Int32 devid, Int32 strmidx);
    
   public delegate Int32 DataCallback(IntPtr merged, Int32 size, Int32 board, Int32 strmidx);

   [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
        static extern Int32 S2253_RegisterCallback(DataCallback cb, Int32 devid, Int32 strmidx);

    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    static extern Int32 S2253_GetHVLock(ref Int32 hvlock, Int32 devid);
    
    [DllImport("mid2253_x64.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
    static extern Int32 S2253_SetPreviewType(MID2253_PREVIEWTYPE type, Int32 devid, Int32 strmidx);
#else
		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_Open(Int32 brdidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_Close(Int32 brdidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetNumDevices(ref Int32 pNumDevices);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetVidSys(MID2253_VIDSYS vidsys, Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetVidSys(ref MID2253_VIDSYS vidsys, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetStatus(ref MID2253STATUS pStatus, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetRecordMode(MID2253_RECMODE recmode, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetRecordMode(ref MID2253_RECMODE recmode, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetMp4Mode(MID2253_MP4MODE mp4mode, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetMp4Mode(ref MID2253_MP4MODE mp4mode, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetAudioEncoding(MID2253_AUDENC aenc, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetAudioEncoding(ref MID2253_AUDENC aenc, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetAudioBitrate(MID2253_AUDIO_BITRATE audbr, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetAudioBitrate(ref MID2253_AUDIO_BITRATE audbr, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartRecord(String filename, Int32 bUnicode, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetLevel(Int32 param, Byte value, Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetLevel(Int32 param, ref Byte value, Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartPreview(Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartAudioPreview(Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StopAudioPreview(Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StopStream(Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetImageSize(Int32 w, Int32 h, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetImageSize(ref Int32 w, ref Int32 h, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetStreamType(Int32 stype, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetStreamType(ref Int32 stype, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetOsd(MID2253_OSDTYPE osdtype, ref MID2253OSD osd, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetClock(ref MID2253CLOCK clk, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetBitrate(Int32 bitrate, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetBitrate(ref Int32 bitrate, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetJpegQ(Int32 q, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetJpegQ(ref Int32 q, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetInterpolateMode(Int32 val, Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetInterpolateMode(ref Int32 val, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartSnapshot(Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetSample(IntPtr data, UInt32 inlen, ref UInt32 outlen, Int32 to, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_EnableSnapshot(Int32 bOn, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetSerialNumber(ref UInt32 serial_number, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetStreamWindow(IntPtr hwnd, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetStreamWindowPosition(ref RECTANGLE rect, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_RepaintWindow(IntPtr hdc, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetGpioInput(ref Int32 value, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetGpioOutput(Int32 value, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetGpioOutput(ref Int32 value, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_WaitGpioInput(MID2253_GPIO_SIGNAL signal, Int32 timout, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetAudioInput(MID2253_AUDIO_INPUT input, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetAudioGain(Boolean bAGC, Int32 gain, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetAudioGain(ref Boolean bAGC, ref Int32 gain, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_FlipImage(Boolean bFlipV, Boolean bFlipH, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetIDR(Int32 val, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_LowLatencyPreview(Boolean bON, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartDecode(String filename, Int32 bUnicode, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StopDecode(Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetNotify(IntPtr hNotifyAPp, UInt32 mNotifyMsg, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_TestDeviceRemoval(Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_TestDecodeDone(Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetOverlay(ref MID2253_OVL_STRUCT ovl, IntPtr data, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetOutputMode(MID2253_OUTPUT_MODE mode, Int32 devid);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetParam(MID2253_PARAM type, ref Int32 val, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetUserData(String data, Int32 len, Int32 interval, Int32 devid, Int32 strmidx);


		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_StartCallback(Int32 devid, Int32 strmidx);

		public delegate Int32 DataCallback(IntPtr merged, Int32 size, Int32 board, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_RegisterCallback(DataCallback cb, Int32 devid, Int32 strmidx);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_GetHVLock(ref Int32 hvlock, Int32 devid);

		[DllImport("mid2253.dll", CallingConvention = CallingConvention.StdCall, SetLastError = true)]
		static extern Int32 S2253_SetPreviewType(MID2253_PREVIEWTYPE type, Int32 devid, Int32 strmidx);

#endif


		private S2253() { }
	}
}