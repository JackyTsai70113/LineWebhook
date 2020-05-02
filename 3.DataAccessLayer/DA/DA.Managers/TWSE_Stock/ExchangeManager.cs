using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Managers.TWSE_Stock;
using Core.Domain.Utility;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.CompilerServices;
using System.Text;
using System.Text.Json;
using System.Web;

namespace DA.Managers.TWSE_Stock {

    public class ExchangeManager : IExchangeManger {

        /// <summary>
        /// 取得玉山金的每日收盤情形
        /// </summary>
        /// <returns></returns>
        public static S2884 Get2884() {
            byte[] bytes = FinancialAndInsurance_DailyQuotes(new DateTime(2020, 4, 30));
            return GetS2884FromJson(bytes);
        }

        /// <summary>
        /// 將byte array 轉為 S2884物件
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <returns>S2884物件</returns>
        private static S2884 GetS2884FromJson(byte[] bytes) {
            JsonDocument doc = JsonDocument.Parse(bytes);
            JsonElement root = doc.RootElement;
            JsonElement S2884_Array = root.GetProperty("data1")[26];

            string directionStr = S2884_Array[9].ToString().StripHTML();
            S2884 stock_2884 = new S2884() {
                TradeVolume = S2884_Array[2].ToString().ThousandToInt(),
                Transaction = S2884_Array[3].ToString().ThousandToInt(),
                TradeValue = S2884_Array[4].ToString().ThousandToInt(),
                OpeningPrice = S2884_Array[5].ToString().ThousandToFloat(),
                HighestPrice = S2884_Array[6].ToString().ThousandToFloat(),
                LowestPrice = S2884_Array[7].ToString().ThousandToFloat(),
                ClosingPrice = S2884_Array[8].ToString().ThousandToFloat(),
                Direction = directionStr.ToStockDirectionEnum(),
                Change = S2884_Array[10].ToString().ThousandToFloat(),

                LastBestBidPrice = S2884_Array[11].ToString().ThousandToFloat(),
                LastBestBidVolume = S2884_Array[12].ToString().ThousandToInt(),
                LastBestAskPrice = S2884_Array[13].ToString().ThousandToFloat(),
                LastBestAskVolume = S2884_Array[14].ToString().ThousandToInt(),
                PriceEarningRatio = S2884_Array[15].ToString().ThousandToFloat()
            };

            return stock_2884;
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