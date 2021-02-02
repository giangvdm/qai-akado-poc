using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using akaSensor.Windows;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace akaFocus.Service
{
	public class Program
	{
		static void LoadLibrary()
		{
			const string strDllPath = "Win32.UtilModule.dll";
			string sDirName = System.IO.Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
			IntPtr handle = IntPtr.Zero;
			string dllPath = Path.Combine(sDirName, IntPtr.Size == 8 ? "x64" : "x86", strDllPath);
			LoadLibraryFlags flags = LoadLibraryFlags.LOAD_WITH_ALTERED_SEARCH_PATH;

			handle = Win32Invoker.LoadLibraryEx(dllPath, IntPtr.Zero, flags);

			if (handle == IntPtr.Zero)
			{
				int nError = Win32Invoker.GetLastError();

				if (nError != 0)
				{
					throw new Win32Exception(nError);
				}
			}
		}

		public static void Main(string[] args)
		{
			LoadLibrary();
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureServices((hostContext, services) =>
				{
					services.AddHostedService<Worker>();
				});
	}
}
