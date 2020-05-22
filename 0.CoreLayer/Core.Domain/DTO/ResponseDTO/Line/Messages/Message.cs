using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages {

    /// <summary>
    /// 訊息
    /// </summary>
    public abstract class Message {

        /// <summary>
        /// [必填] 訊息類型
        /// </summary>
        public string type { get; set; }

        //public string id { get; set; }
    }
}