using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using YahooFinanceApi;

namespace BL.Services.Yahoo {

    public class StockService {

        public StockService() {
            var history = YahooFinanceApi.Yahoo.GetHistoricalAsync("AAPL", new DateTime(2016, 1, 1), new DateTime(2016, 7, 1), Period.Daily).Result;

            //foreach (var candle in history) {
            //    Console.WriteLine($"DateTime: {candle.DateTime}, Open: {candle.Open}, High: {candle.High}, Low: {candle.Low}, Close: {candle.Close}, Volume: {candle.Volume}, AdjustedClose: {candle.AdjustedClose}");
            //}

            //IReadOnlyList<Candle> history2 = YahooFinanceApi.Yahoo.GetHistoricalAsync("2884.TW", new DateTime(2020, 7, 20), new DateTime(2020, 8, 31), Period.Daily).Result;

            //foreach (var candle in history) {
            //    Console.WriteLine($"DateTime: {candle.DateTime}, Open: {candle.Open}, High: {candle.High}, Low: {candle.Low}, Close: {candle.Close}, Volume: {candle.Volume}, AdjustedClose: {candle.AdjustedClose}");
            //}
            //int i = 1;
        }

        public List<object[]> GetSubCandles() {
            IReadOnlyList<Candle> candles = YahooFinanceApi.Yahoo.GetHistoricalAsync("2884.TW", new DateTime(2017, 7, 20), new DateTime(2020, 8, 31), Period.Daily).Result;
            var subCandles = candles.Select(c => new SubCandle {
                Date = c.DateTime,
                Open = c.Open,
                High = c.High,
                Low = c.Low,
                Close = c.Close,
                Value = c.Volume
            }).ToList();
            List<object[]> arr = new List<object[]>();
            foreach (Candle c in candles) {
                object[] objArr = new object[] {
                    c.DateTime,
                    c.Open,
                    c.High,
                    c.Low,
                    c.Close,
                    c.Volume,
                };
                arr.Add(objArr);
            }
            return arr;
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