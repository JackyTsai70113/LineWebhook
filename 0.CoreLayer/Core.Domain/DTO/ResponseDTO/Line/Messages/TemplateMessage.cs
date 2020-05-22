using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    /// <summary>
    /// 模板訊息
    /// </summary>
    public class TemplateMessage : Message {

        public TemplateMessage() {
            type = "template";
        }

        /// <summary>
        /// [必填] 替代文字, Max character limit: 400
        /// </summary>
        public string altText { get; set; }

        /// <summary>
        /// [必填] 模板
        /// </summary>
        public Template template { get; set; }
    }
}