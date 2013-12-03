// 
//  FromFile.cs
//  
//  Author:
//       Simon Mika <simon.mika@imint.se>
//  
//  Copyright (c) 2012-2013 Imint AB
// 
//  All rights reserved.
//
//  Redistribution and use in source and binary forms, with or without modification, are permitted provided that the following conditions are met:
//
//  * Redistributions of source code must retain the above copyright notice, this list of conditions and the following disclaimer.
//  * Redistributions in binary form must reproduce the above copyright notice, this list of conditions and the following disclaimer in
//  the documentation and/or other materials provided with the distribution.
//
//  THIS SOFTWARE IS PROVIDED BY THE COPYRIGHT HOLDERS AND CONTRIBUTORS
//  "AS IS" AND ANY EXPRESS OR IMPLIED WARRANTIES, INCLUDING, BUT NOT
//  LIMITED TO, THE IMPLIED WARRANTIES OF MERCHANTABILITY AND FITNESS FOR
//  A PARTICULAR PURPOSE ARE DISCLAIMED. IN NO EVENT SHALL THE COPYRIGHT OWNER OR
//  CONTRIBUTORS BE LIABLE FOR ANY DIRECT, INDIRECT, INCIDENTAL, SPECIAL,
//  EXEMPLARY, OR CONSEQUENTIAL DAMAGES (INCLUDING, BUT NOT LIMITED TO,
//  PROCUREMENT OF SUBSTITUTE GOODS OR SERVICES; LOSS OF USE, DATA, OR
//  PROFITS; OR BUSINESS INTERRUPTION) HOWEVER CAUSED AND ON ANY THEORY OF
//  LIABILITY, WHETHER IN CONTRACT, STRICT LIABILITY, OR TORT (INCLUDING
//  NEGLIGENCE OR OTHERWISE) ARISING IN ANY WAY OUT OF THE USE OF THIS
//  SOFTWARE, EVEN IF ADVISED OF THE POSSIBILITY OF SUCH DAMAGE.
 
using System;
using Kean.Extension;
using Error = Kean.Error;

namespace Imint.Media.DirectShow.Binding.Filters
{
	public class FromFile :
		Guid
	{
		[System.Runtime.InteropServices.ComVisible(false)]
		[System.Runtime.InteropServices.ComImport, System.Runtime.InteropServices.InterfaceType(System.Runtime.InteropServices.ComInterfaceType.InterfaceIsIUnknown), System.Runtime.InteropServices.Guid("00000001-0000-0000-C000-000000000046")]
		internal interface IClassFactory
		{
			void CreateInstance([System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)] object pUnkOuter, ref System.Guid refiid, [System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)] out object ppunk);
			void LockServer(bool fLock);
		}
		delegate int DllGetClassObject(ref System.Guid ClassId, ref System.Guid InterfaceId, [System.Runtime.InteropServices.Out, System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Interface)] out object ppunk);
		string path;
		public FromFile(System.Guid guid, string path, string description) :
			base(guid, description)
		{
			this.path = path;
		}
		public FromFile(System.Guid guid, string path, string description, params Filters.Abstract[] next) :
			base(guid, description, next)
		{
			this.path = path;
		}
		public override DirectShowLib.IBaseFilter Create()
		{
			DirectShowLib.IBaseFilter result = null;
			try
			{
				IntPtr handle = FromFile.LoadLibrary(this.path);
				IntPtr dllGetClassObjectPointer = FromFile.GetProcAddress(handle, "DllGetClassObject");
				//Convert the function pointer to a .net delegate
				DllGetClassObject dllGetClassObject = (DllGetClassObject)System.Runtime.InteropServices.Marshal.GetDelegateForFunctionPointer(dllGetClassObjectPointer, typeof(DllGetClassObject));
				System.Guid IClassFactoryGUID = new System.Guid("00000001-0000-0000-C000-000000000046"); //IClassFactory class id
				object classFactory;
				System.Guid identifier = this.Identifier;
				if (dllGetClassObject(ref identifier, ref IClassFactoryGUID, out classFactory) == 0)
				{
					System.Guid iBaseFilter = new System.Guid("56a86895-0ad4-11ce-b03a-0020af0ba770");
					object filter;
					(classFactory as IClassFactory).CreateInstance(null, ref iBaseFilter, out filter);
					result = filter as DirectShowLib.IBaseFilter;
				}
			}
			catch (System.Exception e)
			{
				string message = "Filter \"" + this.Description + "\" could not find the file \"" + this.path.ToString() + "\".";
				new DirectShow.Binding.Exception.FilterNotFound(Error.Level.Debug, message, e.Message).Throw();
			}
			if (result.IsNull())
				result = base.Create();
			return result;
		}
		[System.Runtime.InteropServices.DllImport("kernel32.dll", CharSet = System.Runtime.InteropServices.CharSet.Ansi)]
		public static extern IntPtr GetProcAddress(IntPtr hModule, string lpProcName);

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		public static extern bool FreeLibrary(IntPtr hModule);

		[System.Runtime.InteropServices.DllImport("kernel32.dll")]
		public static extern IntPtr LoadLibrary(string lpFileName);
 
	}
}