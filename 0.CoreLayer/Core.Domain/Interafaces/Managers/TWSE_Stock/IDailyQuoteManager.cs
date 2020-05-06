using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Managers.TWSE_Stock {

    public interface IDailyQuoteManager {

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>每日收盤情形列表</returns>
        List<DailyQuote> GetDailyQuoteListByYear(int year, StockCategoryEnum stockCategoryEnum);
    }
}