using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace Core.Domain.Utilities {

    public static class DateTimeUtility {

        /// <summary>
        /// 取得日期月初
        /// </summary>
        /// <returns>日期月初</returns>
        public static DateTime GetFirstDateOfMonth(this DateTime dateTime) {
            return new DateTime(dateTime.Year, dateTime.Month, 1);
        }

        /// <summary>
        /// 取得日期月末
        /// </summary>
        /// <returns>日期月末</returns>
        public static DateTime GetLastDateOfMonth(this DateTime dateTime) {
            int day = DateTime.DaysInMonth(dateTime.Year, dateTime.Month);
            return new DateTime(dateTime.Year, dateTime.Month, day);
        }

        /// <summary>
        /// 取得日期年初(01/01)
        /// </summary>
        /// <returns>日期年初</returns>
        public static DateTime GetFirstDateOfYear(this DateTime dateTime) {
            return new DateTime(dateTime.Year, 1, 1);
        }

        /// <summary>
        /// 取得日期年末(12/31)
        /// </summary>
        /// <returns>日期年末</returns>
        public static DateTime GetLastDateOfYear(this DateTime dateTime) {
            return new DateTime(dateTime.Year, 12, 31);
        }

        /// <summary>
        /// 從時間取得整個月的DateTimeRange
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeRange GetDateTimeRangeByMonth(DateTime dateTime) {
            return new DateTimeRange(dateTime.GetFirstDateOfMonth(), dateTime.GetLastDateOfMonth());
        }

        /// <summary>
        /// 從時間取得整個月的DateTimeRange
        /// </summary>
        /// <param name="dateTime"></param>
        /// <returns></returns>
        public static DateTimeRange GetDateTimeRangeByYear(DateTime dateTime) {
            return new DateTimeRange(dateTime.GetFirstDateOfYear(), dateTime.GetLastDateOfYear());
        }

        /// <summary>
        /// 取得西元年
        /// </summary>
        /// <param name="taiwanYear">民國年</param>
        /// <returns>西元年</returns>
        public static int GetADYear(this int taiwanYear) {
            return taiwanYear + 1911;
        }

        /// <summary>
        /// 取得當月月份
        /// </summary>
        /// <returns>月份</returns>
        public static int GetNowMonth() {
            return DateTime.Now.Month;
        }

        /// <summary>
        /// 取得當年年份
        /// </summary>
        /// <returns>年份</returns>
        public static int GetNowYear() {
            return DateTime.Now.Year;
        }
    }
}