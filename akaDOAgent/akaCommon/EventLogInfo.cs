using Newtonsoft.Json;

namespace akaCommon
{
	public class EventLogInfo : AkaInfoBase
	{
		[JsonProperty(Order = 3)]
		override public string log_type { get => "event";}
		[JsonProperty(Order = 4)]
		// Event in XML to json
		public object Event { get; set; }
	}
}
