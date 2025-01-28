using System.Text.Json.Serialization;

namespace BL.Service.TWSE_Stock
{

    public class TradingVolume
    {
        [JsonPropertyName("stat")]
        public string Stat { get; set; }
        [JsonPropertyName("date")]
        public string Date { get; set; }
        [JsonPropertyName("title")]
        public string Title { get; set; }
        [JsonPropertyName("fields")]
        public string[] Fields { get; set; }
        [JsonPropertyName("data")]
        public string[][] Data { get; set; }
        [JsonPropertyName("notes")]
        public string[] Notes { get; set; }
        [JsonPropertyName("groups")]
        public Group[] Groups { get; set; }
    }

    public class Group
    {
        public int Start { get; set; }
        public int Span { get; set; }
        public string Title { get; set; }
    }
}