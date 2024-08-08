using System.Collections.Generic;
using System.Linq;

namespace DA.Managers.CambridgeDictionary
{
    /// <summary>
    /// 翻譯
    /// </summary>
    public class Translation
    {
        public Translation()
        {
            Means = new List<Mean>();
        }

        /// <summary>
        /// 名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 詞性
        /// </summary>
        public string Speech { get; set; }

        /// <summary>
        /// uk類型
        /// </summary>
        public string UkType { get; set; }

        /// <summary>
        /// uk的KK音標
        /// </summary>
        public string Uk_KKPhonetic { get; set; }

        /// <summary>
        /// us類型
        /// </summary>
        public string UsType { get; set; }

        /// <summary>
        /// us的KK音標
        /// </summary>
        public string Us_KKPhonetic { get; set; }

        /// <summary>
        /// 意思列表
        /// </summary>
        public List<Mean> Means { get; set; }

        /// <summary>
        /// 翻譯輸出文字
        /// </summary>
        public string TranslationStr
        {
            get
            {
                string title = $"{Name} {Speech} {UkType} {Uk_KKPhonetic} {UsType} {Us_KKPhonetic}\n\n";
                string meanStr = string.Join("\n", Means.Select(x => x.MeanStr));
                return title + meanStr;
            }
        }
    }

    /// <summary>
    /// 意思
    /// </summary>
    public class Mean
    {
        public Mean()
        {
            SentenceExamples = new List<SentenceExample>();
        }

        /// <summary>
        /// 英文解釋
        /// </summary>
        public string English { get; set; }

        /// <summary>
        /// 中文意思
        /// </summary>
        public string Chinese { get; set; }

        /// <summary>
        /// 範例列表
        /// </summary>
        public List<SentenceExample> SentenceExamples { get; set; }

        /// <summary>
        /// 意思輸出文字
        /// </summary>
        public string MeanStr
        {
            get
            {
                string SentenceExamplesStr = SentenceExamples.Count > 0 ?
                    $"------------------------\n" +
                    $"{string.Join("", SentenceExamples.Select(x => x.SentenceExampleStr))}" :
                    "";
                return $"{English}\n{Chinese}\n" + SentenceExamplesStr;
            }
        }
    }

    /// <summary>
    /// 例句
    /// </summary>
    public class SentenceExample
    {
        /// <summary>
        /// 例句英文
        /// </summary>
        public string English { get; set; }

        /// <summary>
        /// 例句中文
        /// </summary>
        public string Chinese { get; set; }

        public string SentenceExampleStr
        {
            get
            {
                return $"{English}\n{Chinese}\n";
            }
        }
    }
}