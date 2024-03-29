﻿using BL.Service.Interface;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using isRock.LineBot;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Globalization;
using System.Text;
using System.Text.Json;

namespace BL.Service.TWSE_Stock
{
    public class TradingVolumeService : ITradingVolumeService
    {
        /*
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
        };*/

        /// <summary>
        /// 取資料的筆數
        /// </summary>
        private readonly int _topNumber;

        public TradingVolumeService()
        {
            _topNumber = 100;
        }
        private readonly ILogger<TradingVolumeService> _logger;

        public TradingVolumeService(IConfiguration configuration, ILogger<TradingVolumeService> logger)
        {
            _logger = logger;
            _topNumber = int.Parse(configuration.GetSection("TWSE").GetSection("TradingVolumeNumber").Value);

        }

        public List<MessageBase> GetTradingVolumeStrOverDays(QuerySortTypeEnum querySortType, int days)
        {
            Dictionary<string, int> result = new();
            int count = 0;
            List<DateTime> validDates = new();
            DateTime today = DateTimeUtility.NowDate;
            DateTime date = today;
            while (count < days)
            {
                if (!TryGetTradingVolumeDict(date, querySortType, out Dictionary<string, int> tradingVolumeDict))
                {
                    date = date.AddDays(-1);
                    continue;
                }
                result = GetCombinationTradingVolumeDict(result, tradingVolumeDict);
                count++;
                validDates.Add(date);
                date = date.AddDays(-1);
            }
            switch (querySortType)
            {
                case QuerySortTypeEnum.Ascending:
                    result = result
                        .OrderBy(kvp => kvp.Value)
                        .Take(100)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case QuerySortTypeEnum.Descending:
                    result = result
                        .OrderByDescending(kvp => kvp.Value)
                        .Take(100)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
            }
            string text1 = result.Count == 0 ? "" : GetValidDatesStr(today, days, querySortType, validDates);
            string text2 = GetTradingVolumeStr(result);
            return new List<MessageBase> {
                new TextMessage(text1),
                new TextMessage(text2)
            };
        }

        public List<MessageBase> GetTradingVolumeStr(DateTime date, QuerySortTypeEnum querySortType)
        {
            if (!TryGetTradingVolumeDict(date, querySortType, out Dictionary<string, int> tradingVolumeDict))
            {
                return new List<MessageBase> {
                    new TextMessage($"{date:yyyy/MM/dd} 查無資料")
                };
            }
            string differenceType = GetDifferenceTypeStr(querySortType);
            string textStr = $"以下是{date:yyyy/MM/dd}的綜合{differenceType}股數:\n" +
                                GetTradingVolumeStr(tradingVolumeDict);
            return new List<MessageBase> {
                new TextMessage(textStr)
            };
        }

        private static string GetValidDatesStr(DateTime startDate, int days, QuerySortTypeEnum querySortType, List<DateTime> validDates)
        {
            if (validDates.Count == 0)
            {
                throw new Exception("[GetValidDatesStr] validDates 為空!");
            }
            StringBuilder sb = new();
            string differenceTypeStr = GetDifferenceTypeStr(querySortType);
            sb.Append($"計算自{startDate:yyyy/MM/dd}在{days}天內的綜合{differenceTypeStr}股數:\n");
            foreach (DateTime validDate in validDates)
            {
                sb.Append($"{validDate:yyyy/MM/dd}\n");
            }
            return sb.ToString().TrimEnd();
        }

        private static string GetTradingVolumeStr(Dictionary<string, int> dict)
        {
            if (dict.Count == 0)
            {
                throw new Exception("查無資料");
            }
            StringBuilder sb = new();
            sb.Append("");
            foreach (var kvp in dict)
            {
                sb.Append(kvp.Key + " ");
                sb.Append(kvp.Value + "\n");
            }
            return sb.ToString().TrimEnd();
        }

        private static string GetDifferenceTypeStr(QuerySortTypeEnum querySortType)
        {
            return querySortType switch
            {
                QuerySortTypeEnum.Ascending => "賣超",
                QuerySortTypeEnum.Descending => "買超",
                _ => throw new ArgumentException($"[GetdifferenceTypeStr] 排序類型錯誤! (querySortType: {querySortType})"),
            };
        }

        private bool TryGetTradingVolumeDict(DateTime dateTime, QuerySortTypeEnum querySortType, out Dictionary<string, int> tradingVolumeDict)
        {
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.ForeignInvestors, dateTime,
                    out TradingVolume tradingVolumeOfForeignInvestors) == false)
            {
                tradingVolumeDict = null;
                return false;
            }
            Dictionary<string, int> tradingVolumeDictOfForeignInvestors =
                GetTradingVolumeDictionary(querySortType, tradingVolumeOfForeignInvestors);
            if (TryGetTradingVolume(
                    ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies, dateTime,
                    out TradingVolume tradingVolumeOfSecuritiesInvestmentTrustCompanies) == false)
            {
                tradingVolumeDict = null;
                return false;
            }
            Dictionary<string, int> tradingVolumeDictOfSecuritiesInvestmentTrustCompanies =
                GetTradingVolumeDictionary(querySortType, tradingVolumeOfSecuritiesInvestmentTrustCompanies);
            Dictionary<string, int> combinationTradingVolumeDict = GetCombinationTradingVolumeDict(
                    tradingVolumeDictOfForeignInvestors,
                    tradingVolumeDictOfSecuritiesInvestmentTrustCompanies);
            switch (querySortType)
            {
                case QuerySortTypeEnum.Ascending:
                    tradingVolumeDict = combinationTradingVolumeDict
                        .OrderBy(kvp => kvp.Value)
                        .Take(100)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                case QuerySortTypeEnum.Descending:
                    tradingVolumeDict = combinationTradingVolumeDict
                        .OrderByDescending(kvp => kvp.Value)
                        .Take(100)
                        .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
                    break;
                default:
                    tradingVolumeDict = new Dictionary<string, int>();
                    return false;
            }
            return true;
        }

        private static Dictionary<string, int> GetCombinationTradingVolumeDict(Dictionary<string, int> dict1, Dictionary<string, int> dict2)
        {
            Dictionary<string, int> result = new(dict1);
            foreach (var kvp in dict2)
            {
                if (result.ContainsKey(kvp.Key))
                {
                    result[kvp.Key] += kvp.Value;
                }
                else
                {
                    result.Add(kvp.Key, kvp.Value);
                }
            }
            return result;
        }

        private Dictionary<string, int> GetTradingVolumeDictionary(QuerySortTypeEnum querySortType, TradingVolume tradingVolume)
        {
            string[][] datas = querySortType switch
            {
                QuerySortTypeEnum.Ascending => tradingVolume.Data.Where(d => d[5].StartsWith("-")).ToArray(),
                QuerySortTypeEnum.Descending => tradingVolume.Data.Where(d => !d[5].StartsWith("-")).ToArray(),
                _ => throw new ArgumentException($"[GetTradingVolumeDictionary] 排序類型錯誤! (querySortType: {querySortType})"),
            };
            Dictionary<string, int> dict = new();
            int length = Math.Min(_topNumber, datas.Length);
            for (int i = 0; i < length; i++)
            {
                string name = datas[i][2].Trim();
                int difference = int.Parse(datas[i][5], NumberStyles.AllowLeadingSign | NumberStyles.AllowThousands);

                if (!dict.ContainsKey(name))
                {
                    dict.Add(name, difference);
                }
                else
                {
                    // logger.LogError($"[GetTradingVolumeDictionary] 證券名稱重複, date: {tradingVolume.Date} name: {datas[i][2]}");
                }
            }
            return dict;
        }

        private bool TryGetTradingVolume(
            ForeignAndOtherInvestorEnum foreignAndOtherInvestor, DateTime dateTime, out TradingVolume tradingVolume)
        {
            string twseUri = GetTWSEUrl(foreignAndOtherInvestor);
            string dateStr = dateTime.ToString("yyyyMMdd");
            string uri = twseUri + "?response=json&date=" + dateStr;
            string responseBody = RequestUtility.GetStringFromGetRequest(uri);
            tradingVolume = JsonSerializer.Deserialize<TradingVolume>(responseBody);

            if (tradingVolume.Stat != "OK")
            {
                _logger.LogError("[GetTradingVolumeDict_ForeignInvestorsByJson] 報表未順利取得(), dateTime: {dateTime}", dateTime);
                return false;
            }
            return true;
        }

        /// <summary>
        /// 取得 TWSE 的 Url(Uniform Resource Locator)
        /// </summary>
        /// <param name="foreignAndOtherInvestor"></param>
        /// <returns></returns>
        private static string GetTWSEUrl(ForeignAndOtherInvestorEnum foreignAndOtherInvestor)
        {
            string uri = foreignAndOtherInvestor switch
            {
                ForeignAndOtherInvestorEnum.SecuritiesInvestmentTrustCompanies => "https://www.twse.com.tw/fund/TWT44U",
                ForeignAndOtherInvestorEnum.ForeignInvestors => "https://www.twse.com.tw/fund/TWT38U",
                ForeignAndOtherInvestorEnum.Dealers => "https://www.twse.com.tw/fund/TWT43U",
                _ => throw new Exception("ForeignAndOtherInvestorEnum"),
            };
            return uri;
        }
    }
}