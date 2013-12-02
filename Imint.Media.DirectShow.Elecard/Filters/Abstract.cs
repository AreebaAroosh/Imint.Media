// 
//  Abstract.cs
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
using Kean;
using Kean.Extension;
using Platform = Kean.Platform;

namespace Imint.Media.DirectShow.Elecard.Filters
{
	public abstract class Abstract :
		DirectShow.Binding.Filters.Creator
	{
		System.Guid guid;
		string path;
		string identifier;
		protected global::Elecard.Utilities.Filter backend;
		protected event Action<global::Elecard.Utilities.ModuleConfig> OnPreConfigure;
		public Abstract(string identifier, Guid guid, string path, string description, params DirectShow.Binding.Filters.Abstract[] next) :
			base(description, next)
		{
			this.identifier = identifier;
			this.guid = guid;
			this.path = path;
		}
		public override DirectShowLib.IBaseFilter Create()
		{
			this.backend = null;
			try
			{
				if (this.path.NotEmpty())
					this.backend = global::Elecard.Utilities.Filter.LoadFromFile(this.path, ref this.guid);
				if (this.backend.IsNull())
				{
					this.backend = global::Elecard.Utilities.Filter.LoadFromFile(this.GetComponentPath(this.guid), ref this.guid);
					if (this.backend.IsNull())
						this.backend = new global::Elecard.Utilities.Filter(ref this.guid);
				}
			}
			catch (Exception) { }
			return this.backend.NotNull() ? System.Runtime.InteropServices.Marshal.GetObjectForIUnknown(this.backend.GetIUnknown) as DirectShowLib.IBaseFilter : null;
		}
		protected override bool PreConfiguration(Binding.IBuild build)
		{
			Platform.Application application = build.Application;
			if (application.NotNull())
			{
				Platform.Module module = application["License"];
				if (module is Icop.Client.Module)
				{
					string value = (module as Icop.Client.Module)["media.directshow.elecard." + this.identifier] as string;
					if (value.NotEmpty())
						this.OnPreConfigure += configurator =>
					{
						byte[] key = Convert.FromBase64String(value.Replace('-', '/'));
						byte[] correct = new byte[16];
						byte[] secret = new byte[] { 89, 254, 202, 212, 234, 216, 54, 120, 194, 196, 150, 207, 127, 96, 54, 189 };
						for (int i = 0; i < 16; i++)
							correct[i] = (byte)(secret[i] ^ key[i]);
						Guid activationKey = new Guid(correct);
						//Console.WriteLine(activationKey);
						configurator.SetParamValue(ref activationKey, null);
					};
				}
			}
			if (this.OnPreConfigure.NotNull())
			{
				global::Elecard.Utilities.ModuleConfig moduleConfigurator = this.backend.GetConfigInterface();
				if (moduleConfigurator.NotNull())
				{
					this.OnPreConfigure(moduleConfigurator);
					moduleConfigurator.Dispose();
				}
			}
			return base.PreConfiguration(build);
		}
		protected void Configure(string[] path, params KeyValue<string, int>[] values)
		{
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.CurrentUser;
			foreach (string item in path)
				key = key.OpenSubKey(item, true) ?? key.CreateSubKey(item);

			foreach (KeyValue<string, int> value in values)
				key.SetValue(value.Key, value.Value);
		}
		
		string GetComponentPath(Guid guid)
		{
			string result = "";
			Microsoft.Win32.RegistryKey key = Microsoft.Win32.Registry.ClassesRoot.OpenSubKey(global::Elecard.ElUids.BaseUids.CLSID_RegistryKeyForComponents, false);
			if (key.NotNull())
			{
				Microsoft.Win32.RegistryKey subKey = key.OpenSubKey("{" + guid + "}");
				if (subKey.NotNull())
				{
					result = subKey.GetValue("Path").ToString();
					subKey.Close();
				}
				key.Close();
			}
			return result;
		}
	}
}
