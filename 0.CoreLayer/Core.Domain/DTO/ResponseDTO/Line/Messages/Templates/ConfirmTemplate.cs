using Core.Domain.DTO.ResponseDTO.Line.Messages.Templates.ActionObjects;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.ResponseDTO.Line.Messages.Templates {

    /// <summary>
    /// 確認模板
    /// </summary>
    public class ConfirmTemplate : Template {

        public ConfirmTemplate() {
            type = "confirm";
        }

        /// <summary>
        /// [必填] 文字 (上限 240 字元)
        /// </summary>
        public string text { get; set; }

        /// <summary>
        /// [必填] 點擊動作列表 ( 2 個按鈕的點擊動作)
        /// </summary>
        public List<ActionObject> actions { get; set; }
    }
}