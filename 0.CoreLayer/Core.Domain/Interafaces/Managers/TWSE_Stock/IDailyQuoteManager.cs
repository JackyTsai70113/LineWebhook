using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Managers.TWSE_Stock {

    public interface IDailyQuoteManager {

        /// <summary>
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="GetDailyQuoteListByDay(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByYear(DateTime, StockCategoryEnum)"/> 可以根據 年份 以及 股票分類 取得每日收盤情形列表
        List<DailyQuote> GetDailyQuoteListByMonth(DateTime dateTime, StockCategoryEnum stockCategoryEnum);

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        /// 從 <see cref="GetDailyQuoteListByDay(DateTime, StockCategoryEnum)"/> 可以根據 日期 以及 股票分類 取得每日收盤情形列表
        /// 從 <see cref="GetDailyQuoteListByMonth(DateTime, StockCategoryEnum)"/> 可以根據 月份 以及 股票分類 取得每日收盤情形列表
        List<DailyQuote> GetDailyQuoteListByYear(int year, StockCategoryEnum stockCategoryEnum);
    }
}