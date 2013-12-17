using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Imint.Media.Blackmagic.Test
{
	public class Program
	{
		static void Main(string[] args)
		{
			Kean.Error.Log.CatchErrors = false;
			All.Test();
			Console.WriteLine();
			Console.WriteLine("Press any key to continue");
			Console.ReadKey(true);
		}
	}
}
