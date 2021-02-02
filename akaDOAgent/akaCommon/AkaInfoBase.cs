using Newtonsoft.Json;

namespace akaCommon
{
	public class AkaInfoBase
	{
		[JsonProperty(Order = 1)]
		public string account { get; set; }
		[JsonProperty(Order = 2)]
		public HostInfo host_info { get; set; }
		[JsonProperty(Order = 3)]
		virtual public string log_type { get;}
	}
}
