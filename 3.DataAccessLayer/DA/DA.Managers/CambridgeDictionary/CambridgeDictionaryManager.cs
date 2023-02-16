using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.Utilities;
using DA.Managers.Interfaces;
using System.Collections.Generic;
using System.IO;

namespace DA.Managers.CambridgeDictionary
{

    public class CambridgeDictionaryManager : ICambridgeDictionaryManager
    {

        public List<Translation> CrawlCambridgeDictionary(string vocabulary)
        {
            string url = $"https://dictionary.cambridge.org/zht/%E8%A9%9E%E5%85%B8/%E8%8B%B1%E8%AA%9E-%E6%BC%A2%E8%AA%9E-%E7%B9%81%E9%AB%94/" + vocabulary;

            Stream stream = RequestUtility.GetStreamFromGetRequestAsync(url).Result;
            // Angle Sharp Setting
            IConfiguration configuration = Configuration.Default;
            IBrowsingContext context = BrowsingContext.New(configuration);

            IDocument document = context.OpenAsync(req => req.Content(stream)).Result;

            IHtmlCollection<IElement> prEntryBody__elDivs = document.QuerySelectorAll("div.pr.entry-body__el");

            List<Translation> translations = new List<Translation>();
            foreach (var prEntryBody__elDiv in prEntryBody__elDivs)
            {
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
                IHtmlCollection<IElement> defBlockDdef_blockDivs = posBodyDiv.QuerySelectorAll("div.def-block.ddef_block");
                foreach (IElement defBlockDdef_blockDiv in defBlockDdef_blockDivs)
                {
                    Mean mean = new Mean();
                    // 英文解釋
                    if (defBlockDdef_blockDiv.QuerySelector("div.def.ddef_d.db") != null)
                    {
                        mean.English = defBlockDdef_blockDiv.QuerySelector("div.def.ddef_d.db").TextContent;
                    }
                    // 中文意思
                    if (defBlockDdef_blockDiv.QuerySelector("span.trans.dtrans.dtrans-se") != null)
                    {
                        mean.Chinese = defBlockDdef_blockDiv.QuerySelector("span.trans.dtrans.dtrans-se").TextContent;
                    }

                    IHtmlCollection<IElement> exampDexampDivs = defBlockDdef_blockDiv.QuerySelectorAll("div.examp.dexamp");
                    foreach (IElement exampDexampDiv in exampDexampDivs)
                    {
                        SentenceExample sentenceExample = new SentenceExample();
                        if (exampDexampDiv.QuerySelector("span.eg.deg") != null)
                        {
                            sentenceExample.English = exampDexampDiv.QuerySelector("span.eg.deg").TextContent;
                        }
                        if (exampDexampDiv.QuerySelector("span.trans.dtrans.dtrans-se.hdb") != null)
                        {
                            sentenceExample.Chinese = exampDexampDiv.QuerySelector("span.trans.dtrans.dtrans-se.hdb").TextContent;
                        }
                        mean.SentenceExamples.Add(sentenceExample);
                    }
                    translation.Means.Add(mean);
                }

                #endregion 翻譯與例句

                translations.Add(translation);
            }

            return translations;
        }
    }
}