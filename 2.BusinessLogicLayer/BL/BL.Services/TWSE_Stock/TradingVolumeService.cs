using BL.Services.Excel;
using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.Text;

namespace BL.Services.TWSE_Stock {

    public class TradingVolumeService {
        private readonly ExcelDataReaderService _excelDataReaderService;

        public TradingVolumeService() {
            _excelDataReaderService = new ExcelDataReaderService();
        }

        public TradingVolumeService(ExcelDataReaderService excelDataReaderService) {
            _excelDataReaderService = excelDataReaderService;
        }

        public string GetTradingVolumeStr_ForeignInvestors(DateTime dateTime = default) {
            if (dateTime == default) {
                dateTime = DateTime.Today;
            }
            var dict = GetTradingVolumeDict_ForeignInvestors(dateTime);
            StringBuilder sb = new StringBuilder();
            sb.Append("");
            foreach (var kvp in dict) {
                sb.Append(kvp.Key + " ");
                sb.Append(kvp.Value + "\n");
            }
            return sb.ToString();
        }

        public Dictionary<string, int> GetTradingVolumeDict_ForeignInvestors(DateTime dateTime) {
            var dict = new Dictionary<string, int>();
            try {
                string dateStr = dateTime.ToString("yyyyMMdd");
                string uri = "https://www.twse.com.tw/fund/TWT38U?response=csv&date=" + dateStr;
                DataSet dataSet = _excelDataReaderService.GetDataSetFromUri(uri);

                int startX = 3;
                var table = dataSet.Tables[0];
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
                throw;
            }
        }

        private string TrimQuotationMark(string str) {
            return str.TrimStart('=').Trim('"').Trim();
        }
    }
}