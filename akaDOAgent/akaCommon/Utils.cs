using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Linq;

namespace akaCommon
{
	public static class Utils
	{
		public static long GetUnixEpoch(DateTime dateTime)
		{
			var unixTime = dateTime.ToUniversalTime() - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

			return (long)Math.Round(unixTime.TotalSeconds);
		}

		internal static void CreateFolder(string folderName)
		{
			if (!Directory.Exists(folderName))
			{
				Directory.CreateDirectory(folderName);
			}
		}

		public static bool IsValidUrl(string strUrl)
		{
			string[] ignoreList = new []
			{
				"@yahoo", "@gmail", "@hotmail", "@ymail", "@fsoft", "@fpt",
			};

			if (!string.IsNullOrWhiteSpace(strUrl))
			{
				string sUrlLower = strUrl.ToLower();

				if (ignoreList.Any(x => sUrlLower.Contains(x)))
				{
					return false;
				}

				Match match = Regex.Match(strUrl.Trim(), AppInfo.ValidateUrlPattern);
				return match.Captures.Count > 0;
			}
			return false;
		}
	}
}
