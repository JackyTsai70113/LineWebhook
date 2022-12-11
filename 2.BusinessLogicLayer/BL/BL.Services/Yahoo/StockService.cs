using System;
using System.Collections.Generic;
using YahooFinanceApi;

namespace BL.Services.YahooFinance {

    public class StockService {

        public List<object[]> GetSubCandles() {
            try {
                IReadOnlyList<Candle> candles = Yahoo.GetHistoricalAsync(
                    "2884.TW", new DateTime(2019, 7, 20), new DateTime(2020, 8, 31), Period.Daily).Result;
                List<object[]> arr = new List<object[]>();
                foreach (Candle c in candles) {
                    try {
                        object[] objArr = new object[] {
                            c.DateTime,
                            c.Open,
                            c.High,
                            c.Low,
                            c.Close,
                            c.Volume,
                        };
                        arr.Add(objArr);
                    } catch (Exception) {
                        throw;
                    }
                }
                return arr;
            } catch (System.Exception) {
                throw;
            }
        }
    }

    public class SubCandle {
        public DateTime Date { get; set; }
        public decimal Open { get; set; }
        public decimal High { get; set; }
        public decimal Low { get; set; }
        public decimal Close { get; set; }
        public long Value { get; set; }
    }
}