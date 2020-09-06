using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using DA.Managers.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace DA.Managers.TWSE_Stock {

    public class DailyQuoteManager : IDailyQuoteManager {

        /// <summary>
        /// 根據 日期 以及 股票分類 抓取每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        public List<DailyQuote> CrawlDailyQuoteListByDate(DateTime dateTime,
                                                          StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();
            byte[] bytes = BytesFromDailyQuotesAPIAsync(dateTime, stockCategoryEnum).Result;
            result.AddRange(GetDailyQuoteListFromBytes(bytes));

            return result;
        }

        /// <summary>
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByDate(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        public List<DailyQuote> CrawlDailyQuoteListByMonth(DateTime dateTime,
                                                         StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();

            Stopwatch sw1 = new Stopwatch();
            sw1.Start();
            IEnumerable<DateTime> DateTimeEnumerable = dateTime.GetDateTimeEnumerableByMonthBeforeNow().EachDay();
            List<Task<byte[]>> taskList = new List<Task<byte[]>>();
            foreach (DateTime dt in DateTimeEnumerable) {
                taskList.Add(BytesFromDailyQuotesAPIAsync(dt.Date, stockCategoryEnum));
            }

            var bytesList = Task.WhenAll(taskList).Result;
            foreach (var bytes in bytesList) {
                try {
                    if (bytes == null) {
                        continue;
                    }
                    result.AddRange(GetDailyQuoteListFromBytes(bytes));
                } catch (Exception ex) {
                    Console.WriteLine(
                        $"GetDailyQuoteListByYear 失敗" +
                        $"stockCategoryEnum: {stockCategoryEnum}" +
                        $"ex: {ex}");
                }
            }
            return result;
        }

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByDate(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="CrawlDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        public List<DailyQuote> CrawlDailyQuoteListByYear(int year,
                                                        StockCategoryEnum stockCategoryEnum = StockCategoryEnum.FinancialAndInsurance) {
            List<DailyQuote> result = new List<DailyQuote>();

            IEnumerable<DateTime> DateTimeEnumerable = year.GetDateTimeRangeByYearBeforeNow().EachWeekendDay();
            List<Task<byte[]>> taskList = new List<Task<byte[]>>();
            foreach (DateTime dt in DateTimeEnumerable) {
                taskList.Add(BytesFromDailyQuotesAPIAsync(dt.Date, stockCategoryEnum));
            }

            var bytesList = Task.WhenAll(taskList).Result;
            foreach (var bytes in bytesList) {
                try {
                    if (bytes == null) {
                        continue;
                    }
                    result.AddRange(GetDailyQuoteListFromBytes(bytes));
                } catch (Exception ex) {
                    Console.WriteLine(
                        $"GetDailyQuoteListByYear 失敗" +
                        $"stockCategoryEnum: {stockCategoryEnum}" +
                        $"ex: {ex}");
                }
            }

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

            if (root.GetProperty("stat").ToString() != "OK") {
                // 沒有符合條件的資料
                return new List<DailyQuote>();
            }

            JsonElement dateElement = root.GetProperty("params").GetProperty("date");
            DateTime date = DateTime.ParseExact(dateElement.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);

            JsonElement stockDataElements = root.GetProperty("data1");
            for (int i = 0; i < stockDataElements.GetArrayLength(); i++) {
                JsonElement dailyQuoteArray = stockDataElements[i];
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
        //private static DailyQuote GetDailyQuoteFromBytes(byte[] bytes, int data1_Index) {
        //    DailyQuote dailyQuote;
        //    JsonDocument doc = JsonDocument.Parse(bytes);
        //    JsonElement root = doc.RootElement;

        //    JsonElement dateElement = root.GetProperty("params").GetProperty("date");
        //    DateTime date = DateTime.ParseExact(dateElement.ToString(), "yyyyMMdd", CultureInfo.InvariantCulture);
        //    JsonElement dailyQuoteArray = root.GetProperty("data1")[data1_Index];
        //    dailyQuote = GetDailyQuote(dailyQuoteArray, date);

        //    return dailyQuote;
        //}

        /// <summary>
        /// 取得 指定時間，指定股票分類 的每日收盤行情Api Response
        /// </summary>
        /// <param name="dateTime">指定時間</param>
        /// <returns>位元組</returns>
        private static byte[] BytesFromDailyQuotesAPI(DateTime dateTime, StockCategoryEnum stockCategory) {
            byte[] bytes;
            string responseType = "json";
            string dateStr = dateTime.ToString("yyyyMMdd");
            string typeStr = ((int)stockCategory).ToString();
            string uri = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?" +
                $"response=" + responseType +
                $"&date=" + dateStr +
                $"&type=" + typeStr;
            try {
                HttpClient client = RequestUtility.GetNewHttpClient();
                //發送請求
                HttpResponseMessage httpResponseMessage = client.GetAsync(uri).Result;

                //檢查回應的伺服器狀態StatusCode是否是200 OK
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) {
                    //取得內容
                    bytes = httpResponseMessage.Content.ReadAsByteArrayAsync().Result;
                } //檢查回應的伺服器狀態StatusCode是否是403 Forbidden
                else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden) {
                    RequestUtility.AddUriIndex();
                    bytes = BytesFromDailyQuotesAPI(dateTime, stockCategory);
                    //throw new WebException("禁止伺服器請求!");
                } else if (httpResponseMessage.StatusCode == HttpStatusCode.ProxyAuthenticationRequired) {
                    RequestUtility.AddUriIndex();
                    bytes = BytesFromDailyQuotesAPI(dateTime, stockCategory);
                    //throw new WebException("缺乏代理伺服器要求的身分驗證憑證!");
                } else if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) {
                    RequestUtility.AddUriIndex();
                    bytes = BytesFromDailyQuotesAPI(dateTime, stockCategory);
                } else {
                    RequestUtility.AddUriIndex();
                    bytes = BytesFromDailyQuotesAPI(dateTime, stockCategory);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                RequestUtility.AddUriIndex();
                bytes = BytesFromDailyQuotesAPI(dateTime, stockCategory);
            }

            return bytes;
        }

        /// <summary>
        /// 取得 指定時間，指定股票分類 的每日收盤行情Api Response
        /// </summary>
        /// <param name="dateTime">指定時間</param>
        /// <returns>位元組</returns>
        private static async Task<byte[]> BytesFromDailyQuotesAPIAsync(DateTime dateTime, StockCategoryEnum stockCategory) {
            string responseType = "json";
            string dateStr = dateTime.ToString("yyyyMMdd");
            string typeStr = ((int)stockCategory).ToString();
            string uri = $"https://www.twse.com.tw/exchangeReport/MI_INDEX?" +
                $"response=" + responseType +
                $"&date=" + dateStr +
                $"&type=" + typeStr;

            byte[] bytes;
            try {
                HttpClient client = RequestUtility.GetNewHttpClient();
                HttpResponseMessage httpResponseMessage = await client.GetAsync(uri);
                if (httpResponseMessage.StatusCode == HttpStatusCode.OK) {
                    //取得內容
                    bytes = await httpResponseMessage.Content.ReadAsByteArrayAsync();
                    //檢查回應的伺服器狀態StatusCode是否是403 Forbidden
                } else if (httpResponseMessage.StatusCode == HttpStatusCode.Forbidden) {
                    RequestUtility.AddUriIndex();
                    bytes = await BytesFromDailyQuotesAPIAsync(dateTime, stockCategory);
                    //throw new WebException("禁止伺服器請求!");
                } else if (httpResponseMessage.StatusCode == HttpStatusCode.ProxyAuthenticationRequired) {
                    RequestUtility.AddUriIndex();
                    bytes = await BytesFromDailyQuotesAPIAsync(dateTime, stockCategory);
                    //throw new WebException("缺乏代理伺服器要求的身分驗證憑證!");
                } else if (httpResponseMessage.StatusCode == HttpStatusCode.BadRequest) {
                    RequestUtility.AddUriIndex();
                    bytes = await BytesFromDailyQuotesAPIAsync(dateTime, stockCategory);
                } else {
                    RequestUtility.AddUriIndex();
                    bytes = await BytesFromDailyQuotesAPIAsync(dateTime, stockCategory);
                }
            } catch (Exception ex) {
                Console.WriteLine(ex.ToString());
                RequestUtility.AddUriIndex();
                bytes = await BytesFromDailyQuotesAPIAsync(dateTime, stockCategory);
            }

            return bytes;
        }

        #region 商業邏輯工具

        /// <summary>
        /// 將 JsonElement 轉換成 每日收盤行情物件
        /// </summary>
        /// <param name="dailyQuoteArray">json 形式的 DailyQuote</param>
        /// <param name="date">日期</param>
        /// <returns>每日收盤行情物件</returns>
        private static DailyQuote GetDailyQuote(JsonElement dailyQuoteArray, DateTime date) {
            DailyQuote dailyQuote = new DailyQuote();

            string stockCode = dailyQuoteArray[0].ToString();
            if (!dailyQuoteArray[2].ToString().TryParse(out int tradeVolume)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[2]}");
            };
            if (!dailyQuoteArray[3].ToString().TryParse(out int transaction)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[3]}");
            };
            if (!dailyQuoteArray[4].ToString().TryParse(out long tradeValue)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[4]}");
            };

            if (!dailyQuoteArray[5].ToString().TryParse(out float openingPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[5]}");
            };
            if (!dailyQuoteArray[6].ToString().TryParse(out float highestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[6]}");
            };
            if (!dailyQuoteArray[7].ToString().TryParse(out float lowestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[7]}");
            };
            if (!dailyQuoteArray[8].ToString().TryParse(out float closingPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[8]}");
            };
            StockDirectionEnum direction = StringUtility.StripHtmlTag(dailyQuoteArray[9].ToString()).ToStockDirectionEnum();
            if (!dailyQuoteArray[10].ToString().TryParse(out float change)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[10]}");
            };
            if (!dailyQuoteArray[11].ToString().TryParse(out float lastBestBidPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[11]}");
            };

            if (!dailyQuoteArray[12].ToString().TryParse(out int lastBestBidVolume)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[12]}");
            };
            if (!dailyQuoteArray[13].ToString().TryParse(out float lastBestAskPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[13]}");
            };

            if (!dailyQuoteArray[14].ToString().TryParse(out int lastBestAskVolume)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), date: {date}, dailyQuoteArray: {dailyQuoteArray}, jsonStr: {dailyQuoteArray[14]}");
            };
            if (!dailyQuoteArray[15].ToString().TryParse(out float priceEarningRatio)) {
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