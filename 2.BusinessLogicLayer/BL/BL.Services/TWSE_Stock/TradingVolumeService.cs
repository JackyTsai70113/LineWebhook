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

        private readonly string[] ignoreStockName = {
            "富邦VIX",

            "FH香港正2",
            "中信中國50正2",
            "元大S&P500正2",
            "元大S&P原油正2",
            "元大台灣50正2",
            "元大美債20正2",
            "元大滬深300正2",
            "國泰20年美債正2",
            "國泰中國A50正2",
            "國泰美國道瓊正2",
            "國泰臺灣加權正2",
            "富邦NASDAQ正2",
            "富邦上証正2",
            "富邦日本正2",
            "富邦恒生國企正2",
            "富邦臺灣加權正2",
            "街口布蘭特油正2",

            "FH香港反1",
            "元大S&P500反1",
            "元大台灣50反1",
            "元大滬深300反1",
            "國泰美國道瓊反1",
            "富邦上証反1",
            "富邦印度反1",
            "富邦恒生國企反1",
            "富邦臺灣加權反1",
        };

        /// <summary>
        /// 取資料的筆數
        /// </summary>
        private readonly int _topNumber;

        public TradingVolumeService() {
            _topNumber = ConfigService.TWSE_TradingVolumeNumber;
            _excelDataReaderService = new ExcelDataReaderService();
        }

        public TradingVolumeService(ExcelDataReaderService excelDataReaderService) {
            _excelDataReaderService = excelDataReaderService;
        }

        public string GetAscTradingVolumeStr(DateTime date) {
            if (!TryGetAscTradingVolumeDict(date, out Dictionary<string, int> tradingVolumeDict)) {
                string dateStr = date.ToString("yyyy-MM-dd");
                return dateStr + " 查無資料";
            }
            return GetTradingVolumeStr(tradingVolumeDict);
        }

        public string GetDescTradingVolumeStr(DateTime date) {
            if (!TryGetDescTradingVolumeDict(date, out Dictionary<string, int> tradingVolumeDict)) {
                string dateStr = date.ToString("yyyy-MM-dd");
                return dateStr + " 查無資料";
            }
            return GetTradingVolumeStr(tradingVolumeDict);
        }

        public string GetAscTradingVolumeStrOverDays(int days) {
            int count = 0;
            Dictionary<string, int> result = new Dictionary<string, int>();
            DateTime date = DateTime.Today;
            while (count < days) {
                if (!TryGetAscTradingVolumeDict(date, out Dictionary<string, int> ascTradingVolumeDict)) {
                    date = date.AddDays(-1);
                    continue;
                }
                result = GetCombinationTradingVolumeDict(result, ascTradingVolumeDict);
                count++;
                date = date.AddDays(-1);
            }
            result = result
                .OrderBy(kvp => kvp.Value)
                .Take(100)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return GetTradingVolumeStr(result);
        }

        public string GetDescTradingVolumeStrOverDays(int days) {
            int count = 0;
            Dictionary<string, int> result = new Dictionary<string, int>();
            DateTime date = DateTime.Today;
            while (count < days) {
                if (!TryGetDescTradingVolumeDict(date, out Dictionary<string, int> descTradingVolumeDict)) {
                    date = date.AddDays(-1);
                    continue;
                }
                result = GetCombinationTradingVolumeDict(result, descTradingVolumeDict);
                count++;
                date = date.AddDays(-1);
            }
            result = result
                .OrderByDescending(kvp => kvp.Value)
                .Take(100)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return GetTradingVolumeStr(result);
        }

        private string GetTradingVolumeStr(Dictionary<string, int> dict) {
            if (dict.Count == 0) {
                throw new Exception("查無資料");
            }
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            foreach (var kvp in dict) {
                sb.Append(kvp.Key + " ");
                sb.Append(kvp.Value + "\n");
            }
            return sb.ToString();
        }

        private bool TryGetAscTradingVolumeDict(DateTime dateTime, out Dictionary<string, int> ascTradingVolumeDict) {
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.ForeignInvestors, dateTime,
                    out TradingVolume tradingVolumeOfForeignInvestors) == false) {
                ascTradingVolumeDict = null;
                return false;
            }
            var tradingVolumeDictOfForeignInvestors =
                GetTradingVolumeDictionary(tradingVolumeOfForeignInvestors, false);
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies, dateTime,
                    out TradingVolume tradingVolumeOfSecuritiesInvestmentTrustCompanies) == false) {
                ascTradingVolumeDict = null;
                return false;
            }
            var tradingVolumeDictOfSecuritiesInvestmentTrustCompanies =
                GetTradingVolumeDictionary(tradingVolumeOfSecuritiesInvestmentTrustCompanies, false);
            ascTradingVolumeDict =
                GetCombinationTradingVolumeDict(
                    tradingVolumeDictOfForeignInvestors,
                    tradingVolumeDictOfSecuritiesInvestmentTrustCompanies)
                .OrderBy(kvp => kvp.Value)
                .Take(100)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return true;
        }

        private bool TryGetDescTradingVolumeDict(DateTime dateTime, out Dictionary<string, int> descTradingVolumeDict) {
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.ForeignInvestors, dateTime,
                    out TradingVolume tradingVolumeOfForeignInvestors) == false) {
                descTradingVolumeDict = null;
                return false;
            }
            var tradingVolumeDictOfForeignInvestors =
                GetTradingVolumeDictionary(tradingVolumeOfForeignInvestors);
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies, dateTime,
                    out TradingVolume tradingVolumeOfSecuritiesInvestmentTrustCompanies) == false) {
                descTradingVolumeDict = null;
                return false;
            }
            var tradingVolumeDictOfSecuritiesInvestmentTrustCompanies =
                GetTradingVolumeDictionary(tradingVolumeOfSecuritiesInvestmentTrustCompanies);
            descTradingVolumeDict =
                GetCombinationTradingVolumeDict(
                    tradingVolumeDictOfForeignInvestors,
                    tradingVolumeDictOfSecuritiesInvestmentTrustCompanies)
                .OrderByDescending(kvp => kvp.Value)
                .Take(100)
                .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
            return true;
        }

        private Dictionary<string, int> GetCombinationTradingVolumeDict(Dictionary<string, int> dict1, Dictionary<string, int> dict2) {
            Dictionary<string, int> result = new Dictionary<string, int>(dict1);
            foreach (var kvp in dict2) {
                if (result.ContainsKey(kvp.Key)) {
                    result[kvp.Key] += kvp.Value;
                } else {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            return result;
        }

        private Dictionary<string, int> GetTradingVolumeDictionary(TradingVolume tradingVolume, bool isDesc = true) {
            string[][] datas;
            if (isDesc) {
                datas = tradingVolume.data.Where(d => !d[5].StartsWith("-")).ToArray();
            } else {
                datas = tradingVolume.data.Where(d => d[5].StartsWith("-")).ToArray();
            }

            Dictionary<string, int> dict = new Dictionary<string, int>();
            int length = Math.Min(_topNumber, datas.Count());
            for (int i = 0; i < length; i++) {
                string name = datas[i][2].Trim();
                int difference = int.Parse(datas[i][5], NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands);

                if (!dict.ContainsKey(name)) {
                    dict.Add(name, difference);
                } else {
                    Serilog.Log.Error($"[GetAscTradingVolumeDictionary] 證券名稱重複, " +
                        $"date: {tradingVolume.date}" +
                        $"name: {datas[i][2]}");
                }
            }
            return dict;
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

        /// <summary>
        /// 取得 TWSE 的 Url(Uniform Resource Locator)
        /// </summary>
        /// <param name="foreignAndOtherInvestor"></param>
        /// <returns></returns>
        private string GetTWSEUrl(ForeignAndOtherInvestorEnum foreignAndOtherInvestor) {
            string uri;
            switch (foreignAndOtherInvestor) {
                case ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies:
                    uri = "https://www.twse.com.tw/fund/TWT44U";
                    break;
                case ForeignAndOtherInvestorEnum.ForeignInvestors:
                    uri = "https://www.twse.com.tw/fund/TWT38U";
                    break;
                case ForeignAndOtherInvestorEnum.Dealers:
                    uri = "https://www.twse.com.tw/fund/TWT43U";
                    break;
                default:
                    throw new Exception("ForeignAndOtherInvestorEnum");
            }
            return uri;
        }
    }
}