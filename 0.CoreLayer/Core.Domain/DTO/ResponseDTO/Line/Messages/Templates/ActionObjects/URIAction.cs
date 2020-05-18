using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects {

    public class URIAction : ActionObject {

        public URIAction() {
            type = "uri";
        }

        public string label { get; set; }

        public string uri { get; set; }
    }
}