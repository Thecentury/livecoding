using System;
using System.IO;
using System.Reflection;

namespace LiveCoding.Extension
{
	internal static class AssemblyResolver
	{
		private static volatile bool _attached;

		public static void Attach()
		{
			if (_attached)
			{
				return;
			}

			_attached = true;

			AppDomain.CurrentDomain.AssemblyResolve += CurrentDomain_AssemblyResolve;
		}

		static Assembly CurrentDomain_AssemblyResolve( object sender, ResolveEventArgs args )
		{
			string location = typeof (AssemblyResolver).Assembly.Location;

			string cleanedName = GetCleanedFileName(args.Name);

			string expectedName = Path.Combine(Path.GetDirectoryName(location), cleanedName);

			if (File.Exists(expectedName))
			{
				return Assembly.LoadFile(expectedName);
			}

			return null;
		}

		private static string GetCleanedFileName(string name)
		{
			string[] parts = name.Split(',');

			return parts[0] + ".dll";
		}
	}
}