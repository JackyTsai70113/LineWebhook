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

            IElement senseBodyDsense_bDiv = document.QuerySelector("div.sense-body.dsense_b");

            IHtmlCollection<IElement> blockDivs = senseBodyDsense_bDiv.QuerySelectorAll("div.def-block.ddef_block");

            List<Translation> translations = new List<Translation>();
            foreach (var blockDiv in blockDivs) {
                Translation translation = new Translation();
                // 英文翻譯
                translation.English = blockDiv.QuerySelector("div.def.ddef_d.db").TextContent;
                // 中文翻譯
                translation.Chinese = blockDiv.QuerySelector("span.trans.dtrans.dtrans-se").TextContent;
                // 例句列表
                IHtmlCollection<IElement> exampDexampDivs = blockDiv.QuerySelectorAll("div.examp.dexamp");
                foreach (IElement exampDexampDiv in exampDexampDivs) {
                    Example example = new Example();
                    example.English = exampDexampDiv.QuerySelector("span.eg.deg").TextContent;
                    example.Chinese = exampDexampDiv.QuerySelector("span.trans.dtrans.dtrans-se.hdb").TextContent;
                    translation.Examples.Add(example);
                }
                translations.Add(translation);
            }

            return translations;
        }
    }
}