using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Managers.TWSE_Stock;
using Core.Domain.Interafaces.Repositories;
using Core.Domain.Interafaces.Services;
using DA.Managers.TWSE_Stock;
using DA.Repositories;
using System;
using System.Collections.Generic;
using System.Text;

namespace BL.Services {

    public class DailyQuoteService : IDailyQuoteService {

        public DailyQuoteService() {
            DailyQuoteRepository = new DailyQuoteRepository();
            DailyQuoteManager = new DailyQuoteManager();
        }

        /// <summary>
        /// IDailyQuoteManager介面
        /// </summary>
        public IDailyQuoteManager DailyQuoteManager { get; set; }

        /// <summary>
        /// IDailyQuoteRepository介面
        /// </summary>
        public IDailyQuoteRepository DailyQuoteRepository { get; set; }

        /// <summary>
        /// 根據 年份 以及 股票分類 取得每日收盤情形列表，並且儲存。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public int GetDailyQuoteListAndSave(int year, StockCategoryEnum stockCategoryEnum) {
            int result;

            List<DailyQuote> dailyQuoteList = DailyQuoteManager.GetDailyQuoteListByYear(year, stockCategoryEnum);
            result = DailyQuoteRepository.InsertDailyQuoteList(dailyQuoteList);

            return result;
        }
    }
}