using Newtonsoft.Json;

namespace akaSensorAgent
{
    public class ItemInfor
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
        public long start_time { get; set; }
    }

    public class ProcessInfo : ItemInfor
    {
        [JsonProperty(Order = 13)]
        public long end_time { get; set; }

        [JsonProperty(Order = 14)]
        public double duration { get; set; }
    }
}