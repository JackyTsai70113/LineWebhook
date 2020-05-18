using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    public class TemplateMessage : Message {

        public TemplateMessage() {
            type = "template";
        }

        public string altText { get; set; }
        public Template template { get; set; }
    }
}