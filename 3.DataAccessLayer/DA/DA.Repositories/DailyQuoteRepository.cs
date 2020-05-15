using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Interafaces.Repositories;
using Core.Domain.Utilities;
using DA.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;
using System.Linq;
using Core.Domain.Enums;

namespace DA.Repositories {

    public class DailyQuoteRepository : BaseRepository<DailyQuote>, IDailyQuoteRepository {

        #region Sql

        private readonly string SELECT_BY_DATE = $"SELECT TOP 1 * FROM DailyQuotes WHERE Date = @Date;";

        private readonly string IF_NOT_EXISTS_THEN_INSERT_ALL =
            $"IF NOT EXISTS (SELECT TOP 1 * FROM DailyQuotes WHERE Date = @Date AND StockCode = @StockCode)" +
            $"BEGIN" +
            $"    PRINT 'FALSE';" +
            $"    INSERT INTO DailyQuotes(" +
            $"        [Date]" +
            $"        , CreateDateTime" +
            $"        , StockCode" +
            $"        , TradeVolume" +
            $"        , [Transaction]" +
            $"        , TradeValue" +
            $"        , OpeningPrice" +
            $"        , HighestPrice" +
            $"        , LowestPrice" +
            $"        , ClosingPrice" +
            $"        , Direction" +
            $"        , Change" +
            $"        , LastBestBidPrice" +
            $"        , LastBestBidVolume" +
            $"        , LastBestAskPrice" +
            $"        , LastBestAskVolume" +
            $"        , PriceEarningRatio)" +
            $"    VALUES(" +
            $"        @Date" +
            $"        , @CreateDateTime" +
            $"        , @StockCode" +
            $"        , @TradeVolume" +
            $"        , @Transaction" +
            $"        , @TradeValue" +
            $"        , @OpeningPrice" +
            $"        , @HighestPrice" +
            $"        , @LowestPrice" +
            $"        , @ClosingPrice" +
            $"        , @Direction" +
            $"        , @Change" +
            $"        , @LastBestBidPrice" +
            $"        , @LastBestBidVolume" +
            $"        , @LastBestAskPrice" +
            $"        , @LastBestAskVolume" +
            $"        , @PriceEarningRatio)" +
            $"END";

        #endregion Sql

        /// <summary>
        /// Insert每日收盤行情
        /// </summary>
        /// <param name="dailyQuote">每日收盤行情</param>
        /// <returns>是否成功</returns>
        public bool InsertDailyQuote(DailyQuote dailyQuote = null) {
            try {
                using (SqlConnection sqlConnection = GetSqlConnection()) {
                    sqlConnection.Open();
                    int affectedRowNumber = sqlConnection.Execute(IF_NOT_EXISTS_THEN_INSERT_ALL, dailyQuote);
                    if (affectedRowNumber > 0)
                        return true;
                }
                return false;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Insert每日收盤行情列表
        /// </summary>
        /// <param name="dailyQuoteList">每日收盤行情列表</param>
        /// <returns>成功數量</returns>
        public int InsertDailyQuoteList(List<DailyQuote> dailyQuoteList = null) {
            try {
                int affectedRowNumber = -1;
                OpenSqlConnection();
                affectedRowNumber = Execute(IF_NOT_EXISTS_THEN_INSERT_ALL, dailyQuoteList);
                return affectedRowNumber;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// 根據 日期 取得每日收盤情形列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>每日收盤情形列表</returns>
        public DailyQuote SelectFirstDailyQuoteByDate(DateTime date) {
            DailyQuote dailyQuote;

            string cmdText =
                $"SELECT TOP 1 " +
                $"  * " +
                $"FROM DailyQuotes WITH(NOLOCK)" +
                $"WHERE date = @date";
            dailyQuote = QueryFirst(cmdText, new { date });

            return dailyQuote;
        }

        /// <summary>
        /// 根據 日期，股票代號 取得每日收盤情形物件
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="stockCode">股票代號</param>
        /// <returns>每日收盤情形物件</returns>
        public DailyQuote SelectFirstDailyQuoteByDateAndStockCode(DateTime date, string stockCode) {
            DailyQuote dailyQuote = null;

            string cmdText =
                @"SELECT TOP 1
                    *
                  FROM DailyQuotes WITH(NOLOCK)
                  WHERE Date = @date
                  AND StockCode = @stockCode";

            dailyQuote = QueryFirst(cmdText, new {
                date,
                stockCode
            });

            return dailyQuote;
        }

        /// <summary>
        /// 根據 日期 取得每日收盤情形列表
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>每日收盤情形列表</returns>
        public List<DailyQuote> SelectDailyQuoteByDate(DateTime date) {
            List<DailyQuote> dailyQuoteList;

            string cmdText =
                $"SELECT * " +
                $"FROM DailyQuotes WITH(NOLOCK)" +
                $"WHERE date = @date";
            dailyQuoteList = QueryEnumerable(cmdText, new { date }).ToList();
            return dailyQuoteList;
        }

        ///// <summary>
        ///// Insert每日收盤行情列表
        ///// </summary>
        ///// <param name="dailyQuoteList">每日收盤行情列表</param>
        ///// <returns>成功數量</returns>
        //public DailyQuote GetDailyQuoteListByDateAnd(List<DailyQuote> dailyQuoteList = null) {
        //    try {
        //        int affectedRowNumber = -1;
        //        using (SqlConnection sqlConnection = GetSqlConnection()) {
        //            sqlConnection.Open();
        //            affectedRowNumber = sqlConnection.Execute(IF_NOT_EXISTS_THEN_INSERT_ALL, dailyQuoteList);
        //            return affectedRowNumber;
        //        }
        //    } catch (Exception ex) {
        //        throw ex;
        //    }
        //}
    }
}