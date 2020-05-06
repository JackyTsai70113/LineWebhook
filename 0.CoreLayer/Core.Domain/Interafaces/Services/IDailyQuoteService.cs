using Core.Domain.Enums;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Services {

    public interface IDailyQuoteService {

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表，並且儲存。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        int GetDailyQuoteListAndSave(int year, StockCategoryEnum stockCategoryEnum);
    }
}