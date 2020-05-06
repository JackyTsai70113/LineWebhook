using Core.Domain.Entities.TWSE_Stock.Exchange;
using System;
using System.Collections.Generic;
using System.Text;

namespace Core.Domain.Interafaces.Repositories {

    public interface IDailyQuoteRepository {

        /// <summary>
        /// Insert每日收盤行情
        /// </summary>
        /// <param name="dailyQuote">每日收盤行情</param>
        /// <returns>是否成功</returns>
        bool InsertDailyQuote(DailyQuote dailyQuote = null);

        /// <summary>
        /// Insert每日收盤行情列表
        /// </summary>
        /// <param name="dailyQuoteList">每日收盤行情列表</param>
        /// <returns>成功數量</returns>
        int InsertDailyQuoteList(List<DailyQuote> dailyQuoteList = null);
    }
}