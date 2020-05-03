using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Domain.Utility {

    public static class DateTimeUtility {

        /// <summary>
        /// 取得當年年份
        /// </summary>
        /// <returns>年份</returns>
        public static int GetNowYear() {
            return DateTime.Now.Year;
        }

        public static int GetADYear(this DateTime taiwanDateTime) {
            TaiwanCalendar taiwanCalendar = new TaiwanCalendar();
            return taiwanCalendar.GetYear(taiwanDateTime);
        }

        public static int GetADYear(this int taiwanYear) {
            return taiwanYear + 1911;
        }
    }
}