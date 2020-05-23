using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.Utilities;
using DA.Managers.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DA.Managers.CambridgeDictionary {

    public class CambridgeDictionaryManager : ICambridgeDictionaryManager {

        public Core.Domain.DTO.RequestDTO.CambridgeDictionary CrawlCambridgeDictionary(string vocabulary) {
            string url = $"https://dictionary.cambridge.org/zht/%E8%A9%9E%E5%85%B8/%E8%8B%B1%E8%AA%9E-%E6%BC%A2%E8%AA%9E-%E7%B9%81%E9%AB%94/" + vocabulary;

            string contentStr = RequestUtility.GetStringFromGetRequest(url);
            Stream stream = RequestUtility.GetStreamFromGetRequestAsync(url).Result;
            // Angle Sharp Setting
            IConfiguration configuration = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(configuration);

            IDocument document = context.OpenAsync(req => req.Content(stream)).Result;
            //從第3個Table開始爬
            IElement senseBodyDsense_bDiv = document.QuerySelector("div.sense-body.dsense_b");
            //var defBlockDdefBlockDiv = document.QuerySelectorAll("div.def-block ddef_block ")[];
            //IHtmlCollection<IElement> trElements = defBlockDdefBlockDiv.QuerySelectorAll("tr");

            //for (int i = 3; i < trElements.Length && i < 10 + 3; i++) {
            //    IHtmlCollection<IElement> tdElements = trElements[i].QuerySelectorAll("td");
            //    string yearStr = tdElements[1].InnerHtml.StripHtmlSpace().StripHtmlTag();
            //    int year = 0;
            //    if (yearStr.IndexOf('上') != -1) {
            //        year = int.Parse(yearStr.TrimEnd("年上半年"));
            //    } else if (yearStr.IndexOf('下') != -1) {
            //        year = int.Parse(yearStr.TrimEnd("年下半年"));
            //    } else {
            //        year = int.Parse(yearStr.TrimEnd("年年度"));
            //    }
            //}

            #region 翻譯

            // 英文翻譯
            List<string> englisgTranslationList = senseBodyDsense_bDiv.QuerySelectorAll("div.def.ddef_d.db").Select(x => x.InnerHtml.StripHtmlSpace().StripHtmlTag()).ToList();
            // 中文翻譯
            List<string> chineseTranslationList = senseBodyDsense_bDiv.QuerySelectorAll("span.trans.dtrans.dtrans-se").Select(x => x.InnerHtml).ToList();
            // 翻譯
            List<string[]> translationList = new List<string[]>();
            for (int i = 0; i < englisgTranslationList.Count(); i++) {
                translationList.Add(new string[2] {
                    englisgTranslationList[i],
                    chineseTranslationList[i]
                });
            }

            #endregion 翻譯

            #region 範例

            List<string[]> exampleList = new List<string[]>();
            foreach (var exampDexampDiv in senseBodyDsense_bDiv.QuerySelectorAll("div.examp.dexamp")) {
                string englishExample = exampDexampDiv.QuerySelector("span.eg.deg").InnerHtml.StripHtmlSpace().StripHtmlTag();
                string chineseExample = senseBodyDsense_bDiv.QuerySelector("span.trans.dtrans.dtrans-se").InnerHtml.StripHtmlSpace().StripHtmlTag();
                exampleList.Add(new string[2] {
                    englishExample,
                    chineseExample
                });
            }

            #endregion 範例

            #region 例句

            //範例例句列表
            List<string> exampleSentenceList = new List<string>();
            foreach (IElement item in document.QuerySelector("div.daccord").QuerySelectorAll("li")) {
                exampleSentenceList.Add(item.InnerHtml.StripHtmlSpace().StripHtmlTag());
            }

            #endregion 例句

            Core.Domain.DTO.RequestDTO.CambridgeDictionary cambridgeDictionary =
                new Core.Domain.DTO.RequestDTO.CambridgeDictionary {
                    TranslationList = translationList,
                    ExampleList = exampleList,
                    ExampleSentenceList = exampleSentenceList
                };

            return cambridgeDictionary;
        }
    }
}