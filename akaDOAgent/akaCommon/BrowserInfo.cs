using Newtonsoft.Json;

namespace akaCommon
{
	public class BrowserInfo : AkaInfoBase
	{
		[JsonProperty(Order = 3)]
		override public string log_type { get => "process";}
		[JsonProperty(Order = 4)]
		public string process_name { get; set; }
		[JsonProperty(Order = 5)]
		public string current_url { get; set; }
		[JsonProperty(Order = 6)]
		public string windows_title { get; set; }
		[JsonProperty(Order = 7)]
		public string start_time { get; set; }
	}
}
