using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Managers.TWSE_Stock;
using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;

namespace DA.Managers.TWSE_Stock {

    public class DailyQuoteManager : IDailyQuoteManager {

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        List<DailyQuote> IDailyQuoteManager.GetDailyQuoteListByYear(int year,
                                                                   StockCategoryEnum stockCategoryEnum) {
            return GetDailyQuoteListByYear(year, stockCategoryEnum);
        }

        /// <summary>
        /// 根據 日期 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="GetDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        public static List<DailyQuote> GetDailyQuoteListByDay(DateTime dateTime,
                                                               StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();

            byte[] bytes = BytesFromDailyQuotesAPI(new DateTime(2020, 4, 28), stockCategoryEnum);
            result.AddRange(GetDailyQuoteListFromBytes(bytes));
            //byte[] bytes2 = BytesFromDailyQuotesAPI(new DateTime(year, 4, 30), stockCategoryEnum);
            //result.AddRange(GetDailyQuoteListFromBytes(bytes2));

            return result;
        }

        /// <summary>
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="GetDailyQuoteListByDay(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        public static List<DailyQuote> GetDailyQuoteListByMonth(DateTime dateTime,
                                                               StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();

            byte[] bytes = BytesFromDailyQuotesAPI(new DateTime(2020, 4, 28), stockCategoryEnum);
            result.AddRange(GetDailyQuoteListFromBytes(bytes));
            //byte[] bytes2 = BytesFromDailyQuotesAPI(new DateTime(year, 4, 30), stockCategoryEnum);
            //result.AddRange(GetDailyQuoteListFromBytes(bytes2));

            return result;
        }

        // TODO 把一年中每一天串進去
        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="GetDailyQuoteListByDay(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        public static List<DailyQuote> GetDailyQuoteListByYear(int year,
                                                               StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();

            byte[] bytes = BytesFromDailyQuotesAPI(new DateTime(year, 4, 28), stockCategoryEnum);
            result.AddRange(GetDailyQuoteListFromBytes(bytes));
            var a = GetDailyQuoteFromBytes(bytes, 26);

            return result;
        }

        /// <summary>
        /// 將byte array 轉為 每日收盤行情列表
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <returns>每日收盤行情列表</returns>
        private static List<DailyQuote> GetDailyQuoteListFromBytes(byte[] bytes) {
            List<DailyQuote> dailyQuoteList = new List<DailyQuote>();

            JsonDocument doc = JsonDocument.Parse(bytes);
            JsonElement root = doc.RootElement;
            JsonElement dateElement = root.GetProperty("params").GetProperty("date");
            DateTime date = DateTime.ParseExact(dateElement.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

            JsonElement stockDataElemnts = root.GetProperty("data1");
            for (int i = 0; i < stockDataElemnts.GetArrayLength(); i++) {
                JsonElement dailyQuoteArray = root.GetProperty("data1")[i];
                DailyQuote dailyQuote = GetDailyQuote(dailyQuoteArray, date);
                dailyQuoteList.Add(dailyQuote);
            }

            return dailyQuoteList;
        }

        /// <summary>
        /// 將byte array 轉為 每日收盤行情列表(只取指定資料index)
        /// </summary>
        /// <param name="bytes">byte array</param>
        /// <param name="data1_Index">資料index</param>
        /// <returns>每日收盤行情列表</returns>
        private static DailyQuote GetDailyQuoteFromBytes(byte[] bytes, int data1_Index) {
            DailyQuote dailyQuote;
            JsonDocument doc = JsonDocument.Parse(bytes);
            JsonElement root = doc.RootElement;

            JsonElement dateElement = root.GetProperty("params").GetProperty("date");
            DateTime date = DateTime.ParseExact(dateElement.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
            JsonElement dailyQuoteArray = root.GetProperty("data1")[data1_Index];
            dailyQuote = GetDailyQuote(dailyQuoteArray, date);

            return dailyQuote;
        }

        /// <summary>
        /// 取得指定時間的[金融保險類(17)]每日收盤行情Api Response
        /// </summary>
        /// <param name="dateTime">指定時間</param>
        /// <returns>位元組</returns>
        private static byte[] BytesFromDailyQuotesAPI(DateTime dateTime, StockCategoryEnum stockCategory) {
            byte[] bytes = null;
            string responseType = "json";
            string dateStr = dateTime.ToString("yyyyMMdd");
            string typeStr = ((int)stockCategory).ToString();
            string uri = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?" +
                $"response=" + responseType +
                $"&date=" + dateStr +
                $"&type=" + typeStr;

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

        #region 商業邏輯工具

        /// <summary>
        /// 判斷是否為可轉型的字串
        /// </summary>
        /// <param name="jsonStr">字串</param>
        /// <returns>是否可轉型</returns>
        private static bool IsValidString(string jsonStr) {
            // 空白字串
            if (string.IsNullOrWhiteSpace(jsonStr)) {
                return false;
            }
            // 無值的json回傳值
            if (jsonStr == "--") {
                return false;
            }

            return true;
        }

        /// <summary>
        /// 嘗試轉為int，並取得轉換結果
        /// </summary>
        /// <param name="jsonElement">JsonValue</param>
        /// <param name="intNumber">整數</param>
        /// <returns>是否轉換成功</returns>
        private static bool TryParseInt(JsonElement jsonElement, out int intNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            intNumber = -1;

            try {
                string jsonStr = jsonElement.ToString();

                if (!IsValidString(jsonStr)) {
                    return false;
                }

                intNumber = int.Parse(jsonStr, NumberStyles.AllowThousands);
                result = true;
            } catch (Exception ex) {
                result = false;
                intNumber = -1;
                Console.WriteLine(ex.ToString());
            }
            return result;
        }

        /// <summary>
        /// 嘗試轉為float，並取得轉換結果
        /// </summary>
        /// <param name="jsonElement">JsonValue</param>
        /// <param name="intNumber">整數</param>
        /// <returns>是否轉換成功</returns>
        private static bool TryParseFloat(JsonElement jsonElement, out float floatNumber) {
            bool result;
            // 設定無法正確 Parse 的值
            floatNumber = -1f;

            try {
                string jsonStr = jsonElement.ToString();

                if (!IsValidString(jsonStr)) {
                    return false;
                }

                floatNumber = float.Parse(jsonStr, NumberStyles.AllowThousands | NumberStyles.AllowDecimalPoint);
                result = true;
            } catch (Exception ex) {
                result = false;
                floatNumber = -1f;
            }
            return result;
        }

        /// <summary>
        /// 將 JsonElement 轉換成 DailyQuote
        /// </summary>
        /// <param name="dailyQuoteArray">json 形式的 DailyQuote</param>
        /// <param name="date">日期</param>
        /// <returns>DailyQuote物件</returns>
        private static DailyQuote GetDailyQuote(JsonElement dailyQuoteArray, DateTime date) {
            DailyQuote dailyQuote = new DailyQuote();

            string stockCode = dailyQuoteArray[0].ToString();
            if (!TryParseInt(dailyQuoteArray[2], out int tradeVolume)) {
                Console.WriteLine($"TryParseInt 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[2]}");
            };
            if (!TryParseInt(dailyQuoteArray[3], out int transaction)) {
                Console.WriteLine($"TryParseInt 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[3]}");
            };
            if (!TryParseInt(dailyQuoteArray[4], out int tradeValue)) {
                Console.WriteLine($"TryParseInt 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[4]}");
            };

            if (!TryParseFloat(dailyQuoteArray[5], out float openingPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[5]}");
            };
            if (!TryParseFloat(dailyQuoteArray[6], out float highestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[6]}");
            };
            if (!TryParseFloat(dailyQuoteArray[7], out float lowestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[7]}");
            };
            if (!TryParseFloat(dailyQuoteArray[8], out float closingPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[8]}");
            };
            StockDirectionEnum direction = dailyQuoteArray[9].ToString().StripHtmlTag().ToStockDirectionEnum();
            if (!TryParseFloat(dailyQuoteArray[10], out float change)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[10]}");
            };
            if (!TryParseFloat(dailyQuoteArray[11], out float lastBestBidPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[11]}");
            };

            if (!TryParseInt(dailyQuoteArray[12], out int lastBestBidVolume)) {
                Console.WriteLine($"TryParseInt 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[12]}");
            };
            if (!TryParseFloat(dailyQuoteArray[13], out float lastBestAskPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[13]}");
            };

            if (!TryParseInt(dailyQuoteArray[14], out int lastBestAskVolume)) {
                Console.WriteLine($"TryParseInt 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[14]}");
            };
            if (!TryParseFloat(dailyQuoteArray[15], out float priceEarningRatio)) {
                Console.WriteLine($"TryParseFloat 失敗, date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[15]}");
            };

            dailyQuote.Date = date;
            dailyQuote.StockCode = stockCode;
            dailyQuote.TradeVolume = tradeVolume;
            dailyQuote.Transaction = transaction;
            dailyQuote.TradeValue = tradeValue;
            dailyQuote.OpeningPrice = openingPrice;
            dailyQuote.HighestPrice = highestPrice;
            dailyQuote.LowestPrice = lowestPrice;
            dailyQuote.ClosingPrice = closingPrice;
            dailyQuote.Direction = direction;
            dailyQuote.Change = change;
            dailyQuote.LastBestBidPrice = lastBestBidPrice;
            dailyQuote.LastBestBidVolume = lastBestBidVolume;
            dailyQuote.LastBestAskPrice = lastBestAskPrice;
            dailyQuote.LastBestAskVolume = lastBestAskVolume;
            dailyQuote.PriceEarningRatio = priceEarningRatio;

            return dailyQuote;
        }

        #endregion 商業邏輯工具
    }
}