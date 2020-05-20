using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects {

    public class PostbackAction : ActionObject {

        public PostbackAction() {
            type = "postback";
        }

        public string label { get; set; }
        public string data { get; set; }
        //public string displayText { get; set; }
        //public string text { get; set; }
    }
}