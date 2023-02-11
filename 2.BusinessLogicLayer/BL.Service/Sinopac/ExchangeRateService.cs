using System.Text.Json;
using BL.Service.Interface;
using Core.Domain.DTO.Sinopac;
using Core.Domain.Utilities;

namespace BL.Service.Sinopac
{

    public class ExchangeRateService : IExchangeRateService {

        /// <summary>
        /// 取得換匯資訊
        /// </summary>
        /// <param name="bankBuyingRate">銀行買入匯率</param>
        /// <param name="bankSellingRate">銀行賣出匯率</param>
        /// <param name="quotedDateTime">報價時間</param>
        public void GetExchangeRate(out double bankBuyingRate, out double bankSellingRate,
            out DateTime quotedDateTime) {

            List<ExchangeRate> exchangeRates = GetExchangeRate();
            Info info = exchangeRates[0].SubInfo[0];
            bankBuyingRate = double.Parse(info.DataValue2);
            bankSellingRate = double.Parse(info.DataValue3);

            string titleInfo = exchangeRates[0].TitleInfo;
            titleInfo = StringUtility.StripHtmlTag(titleInfo);
            string quoteDatetimeStr = titleInfo.Substring(5, 19);
            quotedDateTime = DateTime.Parse(quoteDatetimeStr);
        }

        /// <summary>
        /// 取得匯率列表
        /// </summary>
        /// <returns>匯率列表</returns>
        private static List<ExchangeRate> GetExchangeRate() {
            string url = $"https://mma.sinopac.com/ws/share/rate/ws_exchange.ashx?exchangeType=REMIT";
            string response = RequestUtility.GetStringFromGetRequest(url);
            List<ExchangeRate> exchangeRates = JsonSerializer.Deserialize<List<ExchangeRate>>(response);
            return exchangeRates;
        }
    }
}