using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.DTO.RequestDTO {

    /// <summary>
    /// 劍橋辭典 英語-漢語(繁) 爬蟲回傳資料
    /// </summary>
    public class CambridgeDictionary {

        /// <summary>
        /// 翻譯列表, string[0]: 英文, string[1] 中文
        /// </summary>
        public List<string[]> TranslationList { get; set; }

        /// <summary>
        /// 範例列表, string[0]: 英文, string[1] 中文
        /// </summary>
        public List<string[]> ExampleList { get; set; }

        /// <summary>
        /// 範例例句列表
        /// </summary>
        public List<string> ExampleSentenceList { get; set; }
    }
}