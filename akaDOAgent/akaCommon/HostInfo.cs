using Newtonsoft.Json;

namespace akaCommon
{
	public class HostInfo
	{
		[JsonProperty(Order = 1)]
		public string computer_name { get; set; }
		[JsonProperty(Order = 2)]
		public string mac_address { get; set; }
		[JsonProperty(Order = 3)]
		public string serial_number { get; set; }
		[JsonProperty(Order = 4)]
		public string uuid { get; set; }
		[JsonProperty(Order = 5)]
		public string domain_name { get; set; }
	}
}
