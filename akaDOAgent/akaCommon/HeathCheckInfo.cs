using Newtonsoft.Json;

namespace akaCommon
{
	public class HeathCheckInfo : AkaInfoBase
	{
		[JsonProperty(Order = 3)]
		override public string log_type { get => "health_checked"; }
		[JsonProperty(Order = 4)]
		public string status { get => "good"; }
		[JsonProperty(Order = 5)]
		public string start_time { get; set; }
	}
}
