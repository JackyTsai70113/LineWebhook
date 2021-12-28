using Core.Domain.DTO.TWSE;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;

namespace BL.Services.Providers {
    public static class TwseProvider {

        /// <summary>
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByDate(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        public static List<DateTime> GetHolidaySchedule() {
            string uri = $"https://openapi.twse.com.tw/v1/holidaySchedule/holidaySchedule";
            string apiResult = RequestUtility.GetStringFromGetRequest(uri);

            List<HolidaySchedule> holidaySchedules = 
                JsonUtility.Deserialize<List<HolidaySchedule>>(apiResult);

            CultureInfo culture = new CultureInfo("zh-TW", true);
            culture.DateTimeFormat.Calendar = new TaiwanCalendar();
            List<DateTime> holidayDates =
                holidaySchedules.Select(s => DateTime.ParseExact(s.Date.PadLeft(8, '0'), "yMMdd", culture)).ToList();
            return holidayDates;
        }
    }
}