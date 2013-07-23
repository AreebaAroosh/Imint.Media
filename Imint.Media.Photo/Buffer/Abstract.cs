using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Collection = Kean.Core.Collection;
using Raster = Kean.Draw.Raster;
using Uri = Kean.Core.Uri;
using Kean.Core.Collection.Extension;
using Kean.Core.Extension;

namespace Imint.Media.Photo.Buffer
{
	public abstract class Abstract :
		IDisposable
	{
		protected string[] photoPaths;
		protected bool wrap = false;
		protected int index = 0;
		public int Position;
		public abstract Tuple<int, Raster.Image> Next();
		public int Count { get { return this.photoPaths.Length; } }
		public abstract void Close();
		public void Dispose()
		{			
			this.Close();
		}

		public static Buffer.Abstract Open(Uri.Locator name)
		{
			Buffer.Abstract result;
			string[] photoPaths = GetImageSeries(name);
			// If the series contains more than roughly 40 images, we can't store them all in memory.
			if (photoPaths.Length > 40)
			{
				result = new Buffer.Long(photoPaths);
			}
			else
				result = new Buffer.Short(photoPaths);
			return result;
		}

		static string[] GetImageSeries(Uri.Locator name)
		{
			string[] result;
			MatchCollection matches = Regex.Matches(name.Path.Name, @"()(\d*\d{2})$");
			// If the file has a name ending in 2 or more digits,
			// assume series and get a sorted list of the files in it.
			if (matches.Count > 0)
			{
				string match = matches[0].Groups[1].Value;
				string directory = System.IO.Path.GetDirectoryName(name.PlatformPath);
				result = System.IO.Directory.GetFiles(directory, match + "*.png").Sort();
			}
			else // The filename didn't end in 2 or more digits.
				result = new string[]{ name.PlatformPath };
			return result;
		}
	}
}
