using System.Collections.Generic;

namespace Core.Domain.DTO.Sinopac {

    /// <summary>
    /// 匯率
    /// </summary>
    public class ExchangeRate {
        public string TitleInfo { get; set; }
        public string QueryDate { get; set; }

        //public string HeadInfo { get; set; }
        public List<Info> SubInfo { get; set; }

        public string MemoUrl { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
    }

    /// <summary>
    /// 匯率資訊
    /// </summary>
    public class Info {
        public string DataValue1 { get; set; }
        public string DataValue1Img { get; set; }
        public string DataValue2 { get; set; }
        public string DataValue3 { get; set; }
        public string DataValue4 { get; set; }
    }
}