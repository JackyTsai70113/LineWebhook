using Core.Domain.DTO.TWSE;
using Core.Domain.Utilities;
using System.Text.Json;

namespace BL.Service.Provider {
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
                JsonSerializer.Deserialize<List<HolidaySchedule>>(apiResult);

            List<DateTime> holidayDates =
                holidaySchedules.Select(s =>
                    new DateTime(1911 + int.Parse(s.Date[..3]),
                                int.Parse(s.Date.Substring(3, 2)),
                                int.Parse(s.Date.Substring(5, 2))))
                                .ToList();
            return holidayDates;
        }
    }
}