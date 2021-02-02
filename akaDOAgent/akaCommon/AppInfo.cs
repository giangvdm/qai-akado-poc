using System;
using System.IO;

namespace akaCommon
{
	public static class AppInfo
	{
		private static string _url = "http://endpoint.akarpa.io";
		private static string _dataFolder;
		private static string _databaseFile;
		private static string _validateUrlPattern = @"(https?:\/\/)?(www\.)?[-a-zA-Z0-9@:%._\+~#=]{2,256}\.[a-z]{2,6}\b([-a-zA-Z0-9@:%_\+.~#()?&//=]*)";

		public static string ProgramDataFolder
		{
			get
			{
				if (string.IsNullOrEmpty(_dataFolder))
				{
					string folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "FPTAkaFocus");
					Utils.CreateFolder(folder);
					_dataFolder = folder;
				}

				return _dataFolder;
			}
		}

		public static string LogFolder
		{
			get
			{
				return Path.Combine(ProgramDataFolder, "logs\\");
			}
		}

		public static string DatabaseFile
		{
			get
			{
				if (string.IsNullOrEmpty(_databaseFile))
				{
					string folder = Path.Combine(ProgramDataFolder, "Database");
					Utils.CreateFolder(folder);
					_databaseFile = Path.Combine(folder, "db.dat");
				}
				return _databaseFile;
			}
		}

		public static string akaFocusUrl
		{
			get => _url;
			set => _url = value;
		}

		public static string ValidateUrlPattern
		{
			get => _validateUrlPattern;
			set => _validateUrlPattern = value;
		}
	}
}
