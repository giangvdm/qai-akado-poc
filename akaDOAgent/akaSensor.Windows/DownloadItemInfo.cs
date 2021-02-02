
namespace akaSensor.Windows
{
	class DownloadItemInfo
	{
		public int id { get; set; }
		public string browser { get; set; }
		public string process_name { get; set; }
		public string source { get; set; }
		public string target { get; set; }
		public string transfer_type { get => "download"; }
		public long start_time { get; set; }
		public long end_time { get; set; }
		public long file_size { get; set; }
		public string state { get; set; }
	}
}
