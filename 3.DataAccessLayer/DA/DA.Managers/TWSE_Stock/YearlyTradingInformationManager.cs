using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using DA.Managers.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;

namespace DA.Managers.TWSE_Stock {

    /// <summary>
    /// 年度交易資訊 Manager
    /// </summary>
    public class YearlyTradingInformationManager : IYearlyTradingInformationManager {

        /// <summary>
        /// 根據 股票代號 抓取年度交易資訊列表
        /// </summary>
        /// <param name="stockCodeEnum">股票代號</param>
        /// <returns>年度交易資訊列表</returns>
        public List<YearlyTradingInformation> CrawlYearlyTradingInformation(StockCodeEnum stockCodeEnum) {
            List<YearlyTradingInformation> result = new List<YearlyTradingInformation>();

            string responseType = "json";
            string stockNo = stockCodeEnum.ToStockCode();
            string nowMillisecondsStr = DateTimeUtility.NowMilliseconds.ToString();
            string uri = $"https://www.twse.com.tw/exchangeReport/FMNPTK?" +
                $"response=" + responseType +
                $"&stockNo=" + stockNo +
                $"&_=" + nowMillisecondsStr;

            byte[] byteArray = RequestUtility.GetByteArrayFromGetRequest(uri);

            JsonDocument doc = JsonDocument.Parse(byteArray);
            JsonElement root = doc.RootElement;

            if (root.GetProperty("stat").ToString() != "OK") {
                throw new Exception("沒有符合條件的資料");
            }

            JsonElement jsonElement = root.GetProperty("data");
            for (int i = 0; i < jsonElement.GetArrayLength(); i++) {
                JsonElement yearlyTradingInformationArray = jsonElement[i];
                YearlyTradingInformation yearlyTradingInformation = GetYearlyTradingInformation(yearlyTradingInformationArray);
                if (yearlyTradingInformation == null) {
                    continue;
                }
                yearlyTradingInformation.StockCode = stockCodeEnum.ToStockCode();
                result.Add(yearlyTradingInformation);
            }

            return result;
        }

        /// <summary>
        /// 將 JsonElement 轉換成 年度交易物件
        /// </summary>
        /// <param name="yearlyTradingInformationArray">json 形式的 YearlyTradingInformation</param>
        /// <param name="date">日期</param>
        /// <returns>年度交易物件</returns>
        private static YearlyTradingInformation GetYearlyTradingInformation(JsonElement yearlyTradingInformationArray) {
            YearlyTradingInformation yearlyTradingInformation = new YearlyTradingInformation();

            if (!yearlyTradingInformationArray[0].ToString().TryParse(out short year)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[0]}");
            };
            //換成西元年
            year += 1911;
            if (year < DateTimeUtility.NowYear - 10) {
                Console.WriteLine("超過十年不用計算");
                return null;
            }
            // 成交
            if (!yearlyTradingInformationArray[1].ToString().TryParse(out long tradeVolume)) {
                Console.WriteLine($"TryParseLong 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[1]}");
            };
            if (!yearlyTradingInformationArray[2].ToString().TryParse(out long tradeValue)) {
                Console.WriteLine($"TryParseLong 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[2]}");
            };
            if (!yearlyTradingInformationArray[3].ToString().TryParse(out int transaction)) {
                Console.WriteLine($"TryParseInt 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[3]}");
            };

            //統計及日期
            if (!yearlyTradingInformationArray[4].ToString().TryParse(out float highestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[4]}");
            };
            if (!(year + "/" + yearlyTradingInformationArray[5].ToString()).TryParse(out DateTime highestPriceDate)) {
                Console.WriteLine($"TryParseDateTime 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[5]}");
            };
            if (!yearlyTradingInformationArray[6].ToString().TryParse(out float lowestPrice)) {
                Console.WriteLine($"TryParseFloat 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[6]}");
            };
            if (!(year + "/" + yearlyTradingInformationArray[7].ToString()).TryParse(out DateTime lowestPriceDate)) {
                Console.WriteLine($"TryParseDateTime 失敗.ToString(), yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[7]}");
            };
            if (!yearlyTradingInformationArray[8].ToString().TryParse(out float averageClosingPrice)) {
                Console.WriteLine($"TryParseFloat 失敗, yearlyTradingInformationArray: {yearlyTradingInformationArray}, jsonStr: {yearlyTradingInformationArray[8]}");
            };

            yearlyTradingInformation.Year = year;
            yearlyTradingInformation.TradeVolume = tradeVolume;
            yearlyTradingInformation.TradeValue = tradeValue;
            yearlyTradingInformation.Transaction = transaction;
            yearlyTradingInformation.HighestPrice = highestPrice;
            yearlyTradingInformation.HighestPriceDate = highestPriceDate;
            yearlyTradingInformation.LowestPrice = lowestPrice;
            yearlyTradingInformation.LowestPriceDate = lowestPriceDate;
            yearlyTradingInformation.AverageClosingPrice = averageClosingPrice;

            return yearlyTradingInformation;
        }
    }
}