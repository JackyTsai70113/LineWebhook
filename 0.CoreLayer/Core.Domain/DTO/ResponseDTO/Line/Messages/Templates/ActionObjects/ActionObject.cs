using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects {

    /// <summary>
    /// 動作
    /// </summary>
    public abstract class ActionObject {

        /// <summary>
        /// [必填] 動作類型
        /// </summary>
        public string type { get; set; }
    }
}