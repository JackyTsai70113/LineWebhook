using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;

namespace Core.Domain.DTO.RequestDTO.CambridgeDictionary {

    /// <summary>
    /// 翻譯
    /// </summary>
    public class Translation {

        public Translation() {
            Examples = new List<Example>();
        }

        public string English { get; set; }
        public string Chinese { get; set; }
        public List<Example> Examples { get; set; }

        /// <summary>
        /// 輸出用的文字
        /// </summary>
        public string BlockText {
            get {
                return English + "\n"
                    + Chinese + "\n"
                    + Examples.Select(
                        x => x.English + "\n" + x.Chinese + "\n");
            }
        }
    }

    /// <summary>
    /// 例句
    /// </summary>
    public class Example {

        /// <summary>
        /// 例句英文
        /// </summary>
        public string English { get; set; }

        /// <summary>
        /// 例句中文
        /// </summary>
        public string Chinese { get; set; }
    }
}