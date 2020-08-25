using System.Collections.Generic;
using System.IO;
using System.Text;
using AngleSharp;
using AngleSharp.Dom;
using Core.Domain.DTO.RequestDTO.CambridgeDictionary;
using Core.Domain.DTO.Sinopac;
using Core.Domain.Utilities;
using DA.Managers.Interfaces.Sinopac;
using Newtonsoft.Json;

namespace DA.Managers.Sinopac {
    public class ExchangeRateManager : IExchangeRateManager {
        public List<ExchangeRate> CrawlExchangeRate() {
            string url = $"https://mma.sinopac.com/ws/share/rate/ws_exchange.ashx?exchangeType=REMIT";
            string response = RequestUtility.GetStringFromGetRequest(url);
            List<ExchangeRate> exchangeRates = JsonConvert.DeserializeObject<List<ExchangeRate>>(response);

            // Stream stream = RequestUtility.GetStreamFromGetRequestAsync(url).Result;
            // // Angle Sharp Setting
            // IConfiguration configuration = Configuration.Default;
            // IBrowsingContext context = BrowsingContext.New(configuration);

            // IDocument document = context.OpenAsync(req => req.Content(stream)).Result;
            return exchangeRates;
        }
    }
}