namespace Core.Domain.DTO.TWSE
{

    public class TradingVolume
    {
        public string Stat { get; set; }
        public string Date { get; set; }
        public string Title { get; set; }
        public string[] Fields { get; set; }
        public string[][] Data { get; set; }
        public string[] Notes { get; set; }
        public Group[] Groups { get; set; }
    }

    public class Group
    {
        public int Start { get; set; }
        public int Span { get; set; }
        public string Title { get; set; }
    }
}