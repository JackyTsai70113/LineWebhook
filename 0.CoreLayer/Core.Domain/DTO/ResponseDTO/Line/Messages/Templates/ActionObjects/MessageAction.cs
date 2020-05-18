using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects {

    public class MessageAction : ActionObject {

        public MessageAction() {
            type = "message";
        }

        public string label { get; set; }

        /// <summary>
        /// Max character limit: 300
        /// </summary>
        public string text { get; set; }
    }
}