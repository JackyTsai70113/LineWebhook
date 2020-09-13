using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using BL.Services.Cache;
using Core.Domain.Utilities;
using Newtonsoft.Json;

namespace BL.Services.Holiday {

    public static class HolidayHelper {
        private static List<Holiday> holidays;

        public static bool IsHoliday(DateTime date) {
            if (holidays == null) {
                holidays = GetHolidaysFromRedis();
            }
            string dateStr = date.ToString("yyyyMMdd");
            return holidays.Where(h => h.西元日期 == dateStr).First().是否放假 == "2";
        }

        public static List<Holiday> GetHolidays() {
            //2020年
            string uri = "https://quality.data.gov.tw/dq_download_json.php?nid=14718&md5_url=78eba9e4421f1c9d33149f060533691c";
            //2021年
            //string uri = "https://quality.data.gov.tw/dq_download_json.php?nid=14718&md5_url=23a64db222b152d6435142aa2e4cbe34";

            Stream stream = RequestUtility.GetStreamFromGetRequest(uri);
            StreamReader streamReader = new StreamReader(stream);
            string response = streamReader.ReadToEnd();
            List<Holiday> holidays = JsonConvert.DeserializeObject<List<Holiday>>(response);
            return holidays;
        }

        private static List<Holiday> GetHolidaysFromRedis() {
            return new RedisCacheProvider().Get<List<Holiday>>("Holidays_2020");
        }
    }

    public class Holiday {
        public string 西元日期 { get; set; }
        public string 星期 { get; set; }
        public string 是否放假 { get; set; }
        public string 備註 { get; set; }
    }
}