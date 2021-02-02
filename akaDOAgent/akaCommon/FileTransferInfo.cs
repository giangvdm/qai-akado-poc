using Newtonsoft.Json;
using System.Linq;

namespace akaCommon
{
	public class FileTransferInfo : AkaInfoBase
	{
		[JsonProperty(Order = 3)]
		public override string log_type { get => "filetransfer";}
		[JsonProperty(Order = 4)]
		public string browser { get; set; }
		[JsonProperty(Order = 5)]
		public string process_name { get; set; }
		[JsonProperty(Order = 6)]
		public string source { get; set; }
		[JsonProperty(Order = 7)]
		public string target { get; set; }
		[JsonProperty(Order = 8)]
		public string transfer_type { get; set; }
		[JsonProperty(Order = 9)]
		public long start_time { get; set; }
		[JsonProperty(Order = 10)]
		public long end_time { get; set; }
		[JsonProperty(Order = 11)]
		public long file_size { get; set; }
		[JsonProperty(Order = 12)]
		public string state { get; set; }

		public static bool IsValid(FileTransferInfo fileTransfer)
		{
			if (fileTransfer != null)
			{
				string[] arrConditions =
				{
					fileTransfer.source,
					fileTransfer.target,
					fileTransfer.state,
					fileTransfer.browser,
					fileTransfer.process_name,
				};

				return arrConditions.All(x => !string.IsNullOrEmpty(x));
			}
			else
			{
				return false;
			}
		}
	}
}
