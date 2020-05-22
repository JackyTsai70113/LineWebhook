using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates {

    /// <summary>
    /// 模版
    /// </summary>
    public abstract class Template {

        /// <summary>
        /// [必填] 模版類型
        /// </summary>
        public string type { get; set; }
    }
}