using Newtonsoft.Json;

namespace GuaranteedRate.SextantTests.Config
{
    public class WindowModel
    {
        [JsonProperty("height")]
        public int Height { get; set; } 

        [JsonProperty("name")]
        public string Name { get; set; }

        [JsonProperty("title")]
        public string Title { get; set; }

        [JsonProperty("width")]
        public int Width { get; set; }
    }
}
