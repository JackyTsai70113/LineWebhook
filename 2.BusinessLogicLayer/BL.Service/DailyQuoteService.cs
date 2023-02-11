using BL.Service.Interface.TWSE_Stock;
using Core.Domain.Enums;
using DA.Managers.Interfaces.TWSE_Stock;

namespace BL.Service
{

    public class DailyQuoteService : IDailyQuoteService
    {
        private readonly IDailyQuoteManager DailyQuoteManager;

        public DailyQuoteService(IDailyQuoteManager dailyQuoteManager)
        {
            DailyQuoteManager = dailyQuoteManager;
        }

        /// <summary>
        /// 根據 日期 以及 股票分類 抓取每日收盤情形列表。
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>更新數量</returns>
        public void CrawlDailyQuoteListByDate(DateTime dateTime, StockCategoryEnum stockCategoryEnum)
        {
            _ = DailyQuoteManager.CrawlDailyQuoteListByDate(dateTime, stockCategoryEnum);
        }

        /// <summary>
        /// 根據 月份 以及 股票分類 抓取每日收盤情形列表。
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public void CrawlDailyQuoteListByMonth(DateTime dateTime, StockCategoryEnum stockCategoryEnum)
        {
            _ = DailyQuoteManager.CrawlDailyQuoteListByMonth(dateTime, stockCategoryEnum);
        }

        /// <summary>
        /// 根據 年份 以及 股票分類 抓取每日收盤情形列表。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public void CrawlDailyQuoteListByYear(int year, StockCategoryEnum stockCategoryEnum)
        {
            _ = DailyQuoteManager.CrawlDailyQuoteListByYear(year, stockCategoryEnum);
        }
    }
}