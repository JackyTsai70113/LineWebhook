using Core.Domain.Enums;

namespace BL.Service.Interface.TWSE_Stock
{

    public interface IDailyQuoteService
    {

        /// <summary>
        /// 根據 日期 以及 股票分類 抓取每日收盤情形列表，並且更新資料庫。
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        void CrawlDailyQuoteListByDate(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 月份 以及 股票分類 抓取每日收盤情形列表，並且儲存於資料庫。
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        void CrawlDailyQuoteListByMonth(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 年份 以及 股票分類 抓取每日收盤情形列表，並且儲存於資料庫。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        void CrawlDailyQuoteListByYear(int year, StockCategoryEnum stockCategoryEnum);
    }
}