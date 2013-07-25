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
		protected string[] PhotoPaths { get; set; }
		protected bool Wrap { get; set; }
		public int Position { get; protected set; }
		public int Count { get { return this.PhotoPaths.Length; } }
		protected Abstract()
		{ }
		public abstract Tuple<int, Raster.Image> Next();
		public virtual bool Seek(int position)
		{
			bool result;
			if (result = (position >= 0 && position < this.Count))
				this.Position = position;
			return result;
		}
		public abstract void Close();
		void IDisposable.Dispose()
		{
			this.Close();
		}
		public static Buffer.Abstract Open(Uri.Locator name)
		{
			Buffer.Abstract result;
			string[] photoPaths = GetImageSeries(name);
			// If the series contains more than roughly 40 images, we can't store them all in memory.
			if (photoPaths.Length > 40)
				result = new Buffer.Long(photoPaths);
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
			if (matches.Count == 1)
			{
				string match = matches[0].Groups[1].Value;
				string directory = System.IO.Path.GetDirectoryName(name.PlatformPath);
				result = System.IO.Directory.GetFiles(directory, match + "*.png").Sort();
			}
			else if (matches.Count > 1) // This shouldn't happen, but if it does,
				// panic so we can identify why it happened
			{
				result = null;
				//TODO: throw some exception
			}
			else // The filename didn't end in 2 or more digits, so just take the one file.
				result = new string[] { name.PlatformPath };
			return result;
		}
	}
}
