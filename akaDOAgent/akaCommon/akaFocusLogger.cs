using NLog;
using System;
using System.IO;

namespace akaCommon
{
	public class AkaFocusLogger
	{
		public AkaFocusLogger()
		{
			SetupLogger();
		}


		private static void RenameFile()
		{
			foreach (NLog.Targets.Target target in LogManager.Configuration.AllTargets)
			{
				if (target is NLog.Targets.FileTarget)
				{
					NLog.Targets.FileTarget fileTarget = target as NLog.Targets.FileTarget;
					fileTarget.FileName = Path.Combine(AppInfo.LogFolder, $"aka-focus-{DateTime.Now.ToString("yyyy.MM.dd")}.log");
				}
			}
		}

		public static void SetupLogger()
		{
			LogManager.LoadConfiguration("nlog.config");
			RenameFile();
		}

		public void LogInfo(string strMsg)
		{
			RenameFile();
			Logger log = LogManager.GetCurrentClassLogger();
			log.Info(strMsg);
		}

		public void LogDebug(string strMsg)
		{
			RenameFile();
			Logger log = LogManager.GetCurrentClassLogger();
			log.Debug(strMsg);
		}

		public void LogError(string strMsg, Exception ex)
		{
			RenameFile();
			Logger log = LogManager.GetCurrentClassLogger();
			if(ex == null)
			{
				log.Info(strMsg);
			}
			else
			{
				log.Error(ex, strMsg);
			}
		}

		public static void ConsoleLog(string strLog)
		{
#if DEBUG
			Console.WriteLine(strLog);
#endif
			System.Diagnostics.Debug.WriteLine(strLog);
		}
	}
}
