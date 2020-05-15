using BL.Services.Base;
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

    public class DailyQuoteService : BaseService, IDailyQuoteService {

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
        /// 根據 月份 以及 股票分類 取得每日收盤情形列表，並且儲存。
        /// </summary>
        /// <param name="dateTime">日期，用於取得月份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public int GetDailyQuoteListByMonthAndSave(DateTime dateTime, StockCategoryEnum stockCategoryEnum) {
            int result;

            List<DailyQuote> dailyQuoteList = DailyQuoteManager.GetDailyQuoteListByMonth(dateTime, stockCategoryEnum);
            DailyQuoteRepository.SetSqlConnection(LineWebhookContextConnectionString);
            result = DailyQuoteRepository.InsertDailyQuoteList(dailyQuoteList);

            return result;
        }

        /// <summary>
        /// 根據 年份 以及 股票分類 從api取得每日收盤情形列表，並且儲存。
        /// </summary>
        /// <param name="year">年份</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public int GetDailyQuoteListByYearAndSave(int year, StockCategoryEnum stockCategoryEnum) {
            int result;

            List<DailyQuote> dailyQuoteList = DailyQuoteManager.GetDailyQuoteListByYear(year, stockCategoryEnum);
            DailyQuoteRepository.SetSqlConnection(LineWebhookContextConnectionString);
            result = DailyQuoteRepository.InsertDailyQuoteList(dailyQuoteList);

            return result;
        }

        /// <summary>
        /// 根據 日期 從db取得每日收盤情形列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>每日收盤情形列表</returns>
        public List<DailyQuote> GetDailyQuoteByDate(DateTime date) {
            DailyQuoteRepository.SetSqlConnection(LineWebhookContextConnectionString);
            List<DailyQuote> dailyQuoteList = DailyQuoteRepository.SelectDailyQuoteByDate(date);
            if (dailyQuoteList == null || dailyQuoteList.Count == 0) {
                Console.WriteLine("未取到任何值");
            }
            return dailyQuoteList;
        }

        /// <summary>
        /// 根據 日期 以及 股票編號 從db取得每日收盤情形列表。
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="stockCategoryEnum">股票分類</param>
        /// <returns>儲存數量</returns>
        public DailyQuote GetFirstDailyQuoteByDateAndStockCode(DateTime date, string stockCode) {
            DailyQuoteRepository.SetSqlConnection(LineWebhookContextConnectionString);
            DailyQuote dailyQuote = DailyQuoteRepository
                .SelectFirstDailyQuoteByDateAndStockCode(date, stockCode);
            if (dailyQuote == null) {
                Console.WriteLine("未取到任何值");
            }
            return dailyQuote;
        }
    }
}