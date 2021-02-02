using Newtonsoft.Json;

namespace akaSensorAgent
{
    public class PendingItem
    {
        [JsonProperty(Order = 1)]
        public string browser { get; set; }

        [JsonProperty(Order = 2)]
        public string url { get; set; }

        [JsonProperty(Order = 3)]
        public int branch { get; set; }

        [JsonProperty(Order = 4)]
        public int id { get; set; }

        [JsonProperty(Order = 5)]
        public int cif { get; set; }

        [JsonProperty(Order = 6)]
        public string form_name { get; set; }

        [JsonProperty(Order = 7)]
        public string process_name { get; set; }

        [JsonProperty(Order = 8)]
        public int step { get; set; }

        [JsonProperty(Order = 9)]
        public bool isLaststep { get; set; }

        [JsonProperty(Order = 10)]
        public string Actor { get; set; }

        [JsonProperty(Order = 11)]
        public string user_name { get; set; }

        [JsonProperty(Order = 12)]
        public string start_time { get; set; }

        [JsonProperty(Order = 13)]
        public string end_time { get; set; }

        [JsonProperty(Order = 14)]
        public long duration { get; set; }
    }
}
