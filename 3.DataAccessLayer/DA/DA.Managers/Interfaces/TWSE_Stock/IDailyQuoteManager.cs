using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DA.Managers.Interfaces.TWSE_Stock {

    public interface IDailyQuoteManager {

        /// <summary>
        /// 根據 日期 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        List<DailyQuote> CrawlDailyQuoteListByDate(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByDate(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        List<DailyQuote> CrawlDailyQuoteListByMonth(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="CrawlDailyQuoteListByDate(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="CrawlDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        List<DailyQuote> CrawlDailyQuoteListByYear(int year, StockCategoryEnum stockCategoryEnum);
    }
}