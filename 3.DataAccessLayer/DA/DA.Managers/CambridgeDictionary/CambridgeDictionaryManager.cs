using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
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

        public List<Translation> CrawlCambridgeDictionary(string vocabulary) {
            string url = $"https://dictionary.cambridge.org/zht/%E8%A9%9E%E5%85%B8/%E8%8B%B1%E8%AA%9E-%E6%BC%A2%E8%AA%9E-%E7%B9%81%E9%AB%94/" + vocabulary;

            Stream stream = RequestUtility.GetStreamFromGetRequestAsync(url).Result;
            // Angle Sharp Setting
            IConfiguration configuration = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(configuration);

            IDocument document = context.OpenAsync(req => req.Content(stream)).Result;

            List<Translation> translations = new List<Translation>();
            IHtmlCollection<IElement> prEntryBody__elDivs = document.QuerySelectorAll("div.pr.entry-body__el");
            foreach (var prEntryBody__elDiv in prEntryBody__elDivs) {
                Translation translation = new Translation();

                #region 詞性與音標

                IElement posHeaderDposHDiv = prEntryBody__elDiv.QuerySelector("div.pos-header.dpos-h");
                translation.Name = posHeaderDposHDiv.QuerySelector("span.hw.dhw").TextContent;
                translation.Speech = prEntryBody__elDiv.QuerySelector("span.pos.dpos").TextContent;

                IElement ukDpronIDiv = posHeaderDposHDiv.QuerySelector("span.uk.dpron-i");
                translation.UkType = ukDpronIDiv.QuerySelector("span.region.dreg").TextContent;
                translation.Uk_KKPhonetic = ukDpronIDiv.QuerySelector("span.pron.dpron").TextContent;

                IElement usDpronIDiv = posHeaderDposHDiv.QuerySelector("span.us.dpron-i");
                translation.UsType = usDpronIDiv.QuerySelector("span.region.dreg").TextContent;
                translation.Us_KKPhonetic = usDpronIDiv.QuerySelector("span.pron.dpron").TextContent;

                #endregion 詞性與音標

                #region 翻譯與例句

                IElement posBodyDiv = prEntryBody__elDiv.QuerySelector("div.pos-body");
                IHtmlCollection<IElement> prDsenseDivs = posBodyDiv.QuerySelectorAll("div.pr.dsense");
                foreach (IElement prDsenseDiv in prDsenseDivs) {
                    Mean mean = new Mean();
                    // 英文解釋
                    mean.English = prDsenseDiv.QuerySelector("div.def.ddef_d.db").TextContent;
                    // 中文意思
                    mean.Chinese = prDsenseDiv.QuerySelector("span.trans.dtrans.dtrans-se").TextContent;

                    IHtmlCollection<IElement> exampDexampDivs = prDsenseDiv.QuerySelectorAll("div.examp.dexamp");
                    foreach (IElement exampDexampDiv in exampDexampDivs) {
                        SentenceExample sentenceExample = new SentenceExample();
                        sentenceExample.English = exampDexampDiv.QuerySelector("span.eg.deg").TextContent;
                        sentenceExample.Chinese = exampDexampDiv.QuerySelector("span.trans.dtrans.dtrans-se.hdb").TextContent;
                        mean.SentenceExamples.Add(sentenceExample);
                    }
                    translation.Means.Add(mean);
                }

                #endregion 翻譯與例句

                translations.Add(translation);
            }
            //foreach (var blockDiv in blockDivs) {
            //    Translation translation = new Translation();
            //    // 英文翻譯
            //    translation.English = blockDiv.QuerySelector("div.def.ddef_d.db").TextContent;
            //    // 中文翻譯
            //    translation.Chinese = blockDiv.QuerySelector("span.trans.dtrans.dtrans-se").TextContent;
            //    // 例句列表
            //    IHtmlCollection<IElement> exampDexampDivs = blockDiv.QuerySelectorAll("div.examp.dexamp");
            //    foreach (IElement exampDexampDiv in exampDexampDivs) {
            //        Example example = new Example();
            //        example.English = exampDexampDiv.QuerySelector("span.eg.deg").TextContent;
            //        example.Chinese = exampDexampDiv.QuerySelector("span.trans.dtrans.dtrans-se.hdb").TextContent;
            //        translation.Examples.Add(example);
            //    }
            //    translations.Add(translation);
            //}

            return translations;
        }
    }
}