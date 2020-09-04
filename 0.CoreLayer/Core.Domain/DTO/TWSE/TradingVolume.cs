namespace Core.Domain.DTO.TWSE {

    public class TradingVolume {
        public string stat { get; set; }
        public string date { get; set; }
        public string title { get; set; }
        public string[] fields { get; set; }
        public string[][] data { get; set; }
        public string[] notes { get; set; }
        public Group[] groups { get; set; }
    }

    public class Group {
        public int start { get; set; }
        public int span { get; set; }
        public string title { get; set; }
    }
}