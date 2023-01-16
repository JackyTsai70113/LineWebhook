using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;

namespace BL.Service.Interface.TWSE_Stock {

    public interface IDailyQuoteService {

        /// <summary>
        /// 根據 日期 以及 股票分類 抓取每日收盤情形列表，並且更新資料庫。
        /// </summary>
        /// <param name="dateTime">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>更新數量</returns>
        int CrawlDailyQuoteListAndUpdateByDate(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 月份 以及 股票分類 抓取每日收盤情形列表，並且儲存於資料庫。
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        int CrawlDailyQuoteListAndInsertByMonth(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 年份 以及 股票分類 抓取每日收盤情形列表，並且儲存於資料庫。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        int CrawlDailyQuoteListAndInsertByYear(int year, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 日期 以及 股票編號 從db取得每日收盤情形列表。
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        DailyQuote GetFirstDailyQuoteByDateAndStockCode(DateTime date, string stockCode);

        /// <summary>
        /// 根據 日期 從db取得每日收盤情形列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>每日收盤情形列表</returns>
        List<DailyQuote> GetDailyQuoteByDate(DateTime date);
    }
}