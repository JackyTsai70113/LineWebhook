using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Managers.TWSE_Stock;
using Core.Domain.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;

namespace DA.Managers.TWSE_Stock {

    public class DailyQuoteManager : IDailyQuoteManger {

        /// <summary>
        /// 取得每日收盤情形
        /// </summary>
        /// <returns></returns>
        public static DailyQuote GetDailyQuote() {
            byte[] bytes = FinancialAndInsurance_DailyQuotes(new DateTime(2020, 4, 30));
            DailyQuote dailyQuote = GetDailyQuoteFromBytes(bytes);
            return dailyQuote;
        }

        /// <summary>
        /// 將byte array 轉為 DailyQuote 物件
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <returns>DailyQuote 物件</returns>
        private static DailyQuote GetDailyQuoteFromBytes(byte[] bytes) {
            JsonDocument doc = JsonDocument.Parse(bytes);
            JsonElement root = doc.RootElement;

            JsonElement dateElement = root.GetProperty("params").GetProperty("date");
            DateTime date = DateTime.ParseExact(dateElement.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

            JsonElement dailyQuoteArray = root.GetProperty("data1")[26];
            string directionStr = dailyQuoteArray[9].ToString().StripHtmlTag();
            DailyQuote dailyQuote = new DailyQuote() {
                Date = date,
                StockCode = dailyQuoteArray[0].ToString(),
                TradeVolume = dailyQuoteArray[2].ToString().ThousandToInt(),
                Transaction = dailyQuoteArray[3].ToString().ThousandToInt(),
                TradeValue = dailyQuoteArray[4].ToString().ThousandToInt(),
                OpeningPrice = dailyQuoteArray[5].ToString().ThousandToFloat(),
                HighestPrice = dailyQuoteArray[6].ToString().ThousandToFloat(),
                LowestPrice = dailyQuoteArray[7].ToString().ThousandToFloat(),
                ClosingPrice = dailyQuoteArray[8].ToString().ThousandToFloat(),
                Direction = directionStr.ToStockDirectionEnum(),
                Change = dailyQuoteArray[10].ToString().ThousandToFloat(),

                LastBestBidPrice = dailyQuoteArray[11].ToString().ThousandToFloat(),
                LastBestBidVolume = dailyQuoteArray[12].ToString().ThousandToInt(),
                LastBestAskPrice = dailyQuoteArray[13].ToString().ThousandToFloat(),
                LastBestAskVolume = dailyQuoteArray[14].ToString().ThousandToInt(),
                PriceEarningRatio = dailyQuoteArray[15].ToString().ThousandToFloat()
            };

            return dailyQuote;
        }

        /// <summary>
        /// 取得指定時間的[金融保險類(17)]每日收盤行情Api Response
        /// </summary>
        /// <param name="dateTime">指定時間</param>
        /// <returns>位元組</returns>
        private static byte[] FinancialAndInsurance_DailyQuotes(DateTime dateTime) {
            byte[] bytes = null;
            string responseType = "json";
            string dateStr = dateTime.ToString("yyyyMMdd");
            string stockType = "17";
            string uri = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?" +
                $"response=" + responseType +
                $"&date=" + dateStr +
                $"&type=" + stockType;

            using (HttpClient client = new HttpClient()) {
                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(uri).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) {
                    bytes = httpResponseMessage.Content.ReadAsByteArrayAsync().Result;//取得內容
                }
            }
            return bytes;
        }
    }
}