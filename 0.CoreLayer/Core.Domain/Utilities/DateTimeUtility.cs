﻿using System;

namespace Core.Domain.Utilities
{

    public static class DateTimeUtility
    {

        /// <summary>
        /// UNIX 紀元時間
        /// </summary>
        public static DateTime Unix_Epoch_StartTime => new DateTime(1970, 1, 1);

        /// <summary>
        /// 現在日期
        /// </summary>
        public static DateTime Now => DateTime.UtcNow.AddHours(8);

        /// <summary>
        /// 現在日期
        /// </summary>
        public static DateTime NowDate => Now.Date;

        /// <summary>
        /// 現在月份
        /// </summary>
        public static int NowMonth => Now.Month;

        /// <summary>
        /// 現在年份
        /// </summary>
        public static int NowYear => Now.Year;

        public static int NowROCYear => Now.Year - 1911;

        public static bool IsWeekend(this DateTime dateTime) => dateTime.DayOfWeek == DayOfWeek.Saturday || dateTime.DayOfWeek == DayOfWeek.Sunday;

        /// <summary>
        /// 取得西元年
        /// </summary>
        /// <param name="taiwanYear">民國年</param>
        /// <returns>西元年</returns>
        public static int ToADYear(this int taiwanYear) => taiwanYear + 1911;

        /// <summary>
        /// 自 1970-01-01T00:00:00Z 以來所經過的秒數。
        /// </summary>
        public static long NowMilliseconds => DateTimeOffset.Now.ToUnixTimeMilliseconds();

        /// <summary>
        /// 取得日期月初
        /// </summary>
        /// <returns>日期月初</returns>
        public static DateTime GetFirstDateOfMonth(this DateTime dateTime, int months = 0) => new DateTime(dateTime.Year, dateTime.Month, 1).AddMonths(months);

        /// <summary>
        /// 取得日期月末
        /// </summary>
        /// <returns>日期月末</returns>
        public static DateTime GetLastDateOfMonth(this DateTime dateTime, int months = 0) => dateTime.GetFirstDateOfMonth(months + 1).AddDays(-1);

        /// <summary>
        /// 取得年初日期(01/01)
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>日期</returns>
        public static DateTime GetFirstDateOfYear(this int year) => new DateTime(year, 1, 1);

        /// <summary>
        /// 取得年末日期(12/31)
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>日期</returns>
        public static DateTime GetLastDateOfYear(this int year) => new DateTime(year, 12, 31);

        /// <summary>
        /// 從時間取得整月的DateTimeRange
        /// </summary>
        /// <param name="dateTime">時間</param>
        /// <returns>從時間取得整月的DateTimeRange</returns>
        public static DateTimeRange GetDateTimeRangeByMonth(DateTime dateTime) => new DateTimeRange(dateTime.GetFirstDateOfMonth(), dateTime.GetLastDateOfMonth());

        /// <summary>
        /// 從時間取得不早於現在時間的整月的DateTimeRange
        /// </summary>
        /// <param name="dateTime">時間</param>
        /// <returns>DateTimeRange</returns>
        public static DateTimeRange GetDateTimeRangeByMonthBeforeNow(this DateTime dateTime)
        {
            DateTimeRange dateTimeRange;

            if (NowYear < dateTime.Year || (NowYear == dateTime.Year && NowMonth < dateTime.Month))
            {
                throw new ArgumentException("Input錯誤: 月份不可大於現在月分", dateTime.ToString());
            }

            if (NowYear == dateTime.Year && NowMonth == dateTime.Month)
            {
                //現在時間早於當月最後一天，以現在時間為主
                dateTimeRange = new DateTimeRange(dateTime.GetFirstDateOfMonth(), DateTime.Now);
            }
            else
            {
                //現在時間晚於當月最後一天，以當月最後一天為主
                dateTimeRange = new DateTimeRange(dateTime.GetFirstDateOfMonth(), dateTime.GetLastDateOfMonth());
            }

            return dateTimeRange;
        }

        /// <summary>
        /// 從年份取得整年的DateTimeRange
        /// </summary>
        /// <param name="year">年份</param>
        /// <returns>從時間取得整月的DateTimeRange</returns>
        public static DateTimeRange GetDateTimeRangeByYear(this int year) => new DateTimeRange(year.GetFirstDateOfYear(), year.GetLastDateOfYear());

        /// <summary>
        /// 從時間取得不早於現在時間的整年的DateTimeRange
        /// </summary>
        /// <param name="dateTime">時間</param>
        /// <returns>從時間取得整月的DateTimeRange</returns>
        public static DateTimeRange GetDateTimeRangeByYearBeforeNow(this int year)
        {
            DateTimeRange dateTimeRange;
            if (year > Now.Year)
            {
                throw new ArgumentException("年份不可大於現在年分", year.ToString());
            }

            if (year < Now.Year)
            {
                dateTimeRange = new DateTimeRange(year.GetFirstDateOfYear(), year.GetLastDateOfYear());
            }
            else
            {
                dateTimeRange = new DateTimeRange(year.GetFirstDateOfYear(), Now);
            }

            return dateTimeRange;
        }
    }
}