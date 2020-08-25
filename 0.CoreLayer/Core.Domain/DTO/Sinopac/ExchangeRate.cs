using System;
using System.Collections.Generic;
using System.Text;
using Core.Domain.DTO.ResponseDTO.Line.Messages;

namespace Core.Domain.DTO.Sinopac {

    public class ExchangeRate {
        public string TitleInfo { get; set; }
        public string QueryDate { get; set; }
        //public string HeadInfo { get; set; }
        public List<Info> SubInfo { get; set; }
        public string MemoUrl { get; set; }
        public string Header { get; set; }
        public string Message { get; set; }
    }
    public class Info {
        public string DataValue1 { get; set; }
        public string DataValue1Img { get; set; }
        public string DataValue2 { get; set; }
        public string DataValue3 { get; set; }
        public string DataValue4 { get; set; }
    }
}