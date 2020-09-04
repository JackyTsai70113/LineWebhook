using BL.Services.Excel;
using Core.Domain.DTO.TWSE;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using ExcelDataReader.Log;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Linq;
using System.Text;

namespace BL.Services.TWSE_Stock {

    public class TradingVolumeService {
        private readonly ExcelDataReaderService _excelDataReaderService;

        /// <summary>
        /// 取資料的筆數
        /// </summary>
        private readonly int topNumber = 50;

        public TradingVolumeService() {
            _excelDataReaderService = new ExcelDataReaderService();
            var ddd = "tv ";
            var a = GetTopTradingVolumeAscDict(
                ForeignAndOtherInvestorEnum.ForeignInvestors, DateTime.Today.AddDays(-1), 50);
            var b = GetTopTradingVolumeDescDict(
                ForeignAndOtherInvestorEnum.ForeignInvestors, DateTime.Today.AddDays(-1), 50);
        }

        public TradingVolumeService(ExcelDataReaderService excelDataReaderService) {
            _excelDataReaderService = excelDataReaderService;
        }

        public string GetTradingVolumeStr(Dictionary<string, int> dict) {
            if (dict.Count == 0) {
                return "查無資料";
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            foreach (var kvp in dict) {
                sb.Append(kvp.Key + " ");
                sb.Append(kvp.Value + "\n");
            }
            return sb.ToString();
        }

        /// <summary>
        /// 順向取得 法人買賣金額(Trading Value of Foreign & Other Investors)
        /// </summary>
        /// <param name="foreignAndOtherInvestor">法人Enum(可選擇投信/外資及陸資)</param>
        /// <param name="dateTime">日期</param>
        /// <param name="topNumber">資料筆數</param>
        /// <returns>法人買賣字典<證券名稱, 買賣超股數></returns>
        public Dictionary<string, int> GetTopTradingVolumeAscDict(
            ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime, int topNumber) {
            Dictionary<string, int> dict;
            if (TryGetTradingVolume(foreignAndOtherInvestor, dateTime, out TradingVolume tradingVolume) == false) {
                return null;
            }

            dict = new Dictionary<string, int>();
            for (int i = 0; i < topNumber; i++) {
                string name = tradingVolume.data[i][2].ToString().Trim();
                var fmt = new NumberFormatInfo();
                fmt.NegativeSign = "-";
                int difference = int.Parse(tradingVolume.data[i][5], NumberStyles.AllowThousands, fmt);

                if (!dict.ContainsKey(name)) {
                    dict.Add(name, difference);
                } else {
                    throw new Exception("Key重複");
                }
            }
            return dict;
        }

        /// <summary>
        /// 逆向取得 法人買賣金額(Trading Value of Foreign & Other Investors)
        /// </summary>
        /// <param name="foreignAndOtherInvestor">法人Enum(可選擇投信/外資及陸資)</param>
        /// <param name="dateTime">日期</param>
        /// <param name="topNumber">資料筆數</param>
        /// <returns>法人買賣字典<證券名稱, 買賣超股數></returns>
        public Dictionary<string, int> GetTopTradingVolumeDescDict(
            ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime, int topNumber) {
            Dictionary<string, int> dict;
            if (TryGetTradingVolume(foreignAndOtherInvestor, dateTime, out TradingVolume tradingVolume) == false) {
                return null;
            }

            dict = new Dictionary<string, int>();
            string[] singleData = tradingVolume.data
                .Where(d => {
                    NumberStyles numberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
                    return int.Parse(d[5], numberStyles) < 0;
                }).First();
            int index = tradingVolume.data.ToList().IndexOf(singleData);
            for (int i = index; i < index + topNumber; i++) {
                string name = tradingVolume.data[i][2].ToString().Trim();
                NumberStyles numberStyles = NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands;
                int difference = int.Parse(tradingVolume.data[i][5], numberStyles);

                if (!dict.ContainsKey(name)) {
                    dict.Add(name, difference);
                } else {
                    throw new Exception("Key重複");
                }
            }
            return dict;
        }

        /// <summary>
        /// 取得 法人買賣金額(Trading Value of Foreign & Other Investors)
        /// </summary>
        /// <param name="foreignAndOtherInvestor">法人Enum(可選擇投信/外資及陸資)</param>
        /// <param name="dateTime">日期</param>
        /// <param name="topNumber">取前topNumber個</param>
        /// <returns></returns>
        public Dictionary<string, int> GetTopTradingVolumeDict_ForeignInvestors(
            ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime, int topNumber) {
            var dict = new Dictionary<string, int>();
            try {
                string dateStr = dateTime.ToString("yyyyMMdd");
                string uri = "https://www.twse.com.tw/fund/TWT38U?response=csv&date=" + dateStr;
                DataSet dataSet = _excelDataReaderService.GetDataSetFromUri(uri);

                int startX = 3;
                var table = dataSet.Tables[0];
                if (table.Rows.Count == 1 && table.Columns.Count == 1) {
                    Serilog.Log.Debug(
                        $"[GetTradingVolumeDict_ForeignInvestors] 報表未順利取得()，dateTime: {dateTime}");
                    return null;
                }
                for (int row = 0; row < 50; row++) {
                    //string securityCode = table.Rows[startX + row][1].ToString();
                    //securityCode = TrimQuotationMark(securityCode);
                    string name = table.Rows[startX + row][2].ToString().Trim();
                    int difference = int.Parse(table.Rows[startX + row][5].ToString(), NumberStyles.AllowThousands);

                    //if (!dict.ContainsKey(securityCode)) {
                    //    dict.Add(securityCode, difference);
                    //} else {
                    //    throw new Exception("Key重複");
                    //}
                    if (!dict.ContainsKey(name)) {
                        dict.Add(name, difference);
                    } else {
                        throw new Exception("Key重複");
                    }
                }
                return dict;
            } catch (Exception ex) {
                Serilog.Log.Error("GetTradingVolume_ForeignInvestors" + ex.ToString());
                RequestUtility.AddUriIndex();
                return GetTopTradingVolumeDict_ForeignInvestors(
                    foreignAndOtherInvestor, dateTime, topNumber);
            }
        }

        private bool TryGetTradingVolume(
            ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime, out TradingVolume tradingVolume) {
            string twseUri = GetTWSEUrl(foreignAndOtherInvestor);
            string dateStr = dateTime.ToString("yyyyMMdd");
            string uri = twseUri + "?response=json&date=" + dateStr;
            string responseBody = RequestUtility.GetStringFromGetRequest(uri);
            tradingVolume = JsonConvert.DeserializeObject<TradingVolume>(responseBody);

            if (tradingVolume.stat != "OK") {
                Serilog.Log.Debug(
                    $"[GetTradingVolumeDict_ForeignInvestorsByJson] 報表未順利取得()，dateTime: {dateTime}");
                return false;
            }
            return true;
        }

        private TradingVolume GetTradingVolume(ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime) {
            string twseUri = GetTWSEUrl(foreignAndOtherInvestor);
            string dateStr = dateTime.ToString("yyyyMMdd");
            string uri = twseUri + "?response=json&date=" + dateStr;
            string responseBody = RequestUtility.GetStringFromGetRequest(uri);
            TradingVolume tradingVolume = JsonConvert.DeserializeObject<TradingVolume>(responseBody);
            return tradingVolume;
        }

        /// <summary>
        /// 取得 TWSE 的 Url(Uniform Resource Locator)
        /// </summary>
        /// <param name="foreignAndOtherInvestor"></param>
        /// <returns></returns>
        private string GetTWSEUrl(ForeignAndOtherInvestorEnum foreignAndOtherInvestor) {
            string uri;
            switch (foreignAndOtherInvestor) {
                case ForeignAndOtherInvestorEnum.Dealers:
                    uri = "https://www.twse.com.tw/fund/TWT43U";
                    break;
                case ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies:
                    uri = "https://www.twse.com.tw/fund/TWT44U";
                    break;
                case ForeignAndOtherInvestorEnum.ForeignInvestors:
                    uri = "https://www.twse.com.tw/fund/TWT38U";
                    break;
                default:
                    throw new Exception("ForeignAndOtherInvestorEnum");
            }
            return uri;
        }

        private string TrimQuotationMark(string str) {
            return str.TrimStart('=').Trim('"').Trim();
        }
    }
}