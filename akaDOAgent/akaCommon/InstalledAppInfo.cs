using Newtonsoft.Json;

namespace akaCommon
{
	public class InstalledAppInfo : AkaInfoBase
	{
		[JsonProperty(Order = 3)]
		override public string log_type { get => "installed_program"; }
		[JsonProperty(Order = 4)]
		public string applications { get; set; }
	}
}
