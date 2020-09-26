using System.Collections.Generic;
using System.Text;
using BL.Services.Interfaces;
using Core.Domain.DTO.Sinopac;
using Core.Domain.Utilities;
using isRock.LineBot;
using Newtonsoft.Json;

namespace BL.Services.Sinopac {

    public class ExchangeRateService : IExchangeRateService {

        /// <summary>
        /// 取得LINE訊息列表
        /// </summary>
        /// <returns>LINE訊息列表</returns>
        public List<MessageBase> GetExchangeRateMessage() {
            List<ExchangeRate> exchangeRates = GetExchangeRate();
            Info info = exchangeRates[0].SubInfo[0];
            string titleInfo = exchangeRates[0].TitleInfo;
            titleInfo = StringUtility.StripHtmlTag(titleInfo);
            titleInfo = titleInfo.Substring(0, titleInfo.IndexOf('本'));
            StringBuilder sb = new StringBuilder();
            sb.Append("美金報價\n");
            sb.Append("---------------------\n");
            sb.Append($"({titleInfo})\n");
            sb.Append($"銀行買入：{info.DataValue2}\n");
            sb.Append($"銀行賣出：{info.DataValue3}");

            return new List<MessageBase> {
                new TextMessage(sb.ToString())
            };
        }

        /// <summary>
        /// 取得匯率列表
        /// </summary>
        /// <returns>匯率列表</returns>
        private List<ExchangeRate> GetExchangeRate() {
            string url = $"https://mma.sinopac.com/ws/share/rate/ws_exchange.ashx?exchangeType=REMIT";
            string response = RequestUtility.GetStringFromGetRequest(url);
            List<ExchangeRate> exchangeRates = JsonConvert.DeserializeObject<List<ExchangeRate>>(response);
            return exchangeRates;
        }
    }
}