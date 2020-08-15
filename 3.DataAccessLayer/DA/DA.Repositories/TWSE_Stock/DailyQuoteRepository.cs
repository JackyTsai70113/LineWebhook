﻿using Core.Domain.Entities.TWSE_Stock.Exchange;
using DA.Repositories.Base;
using DA.Repositories.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.Repositories.TWSE_Stock {

    public class DailyQuoteRepository : BaseRepository<DailyQuote>, IDailyQuoteRepository {

        /// <summary>
        /// Insert每日收盤行情列表
        /// </summary>
        /// <param name="dailyQuoteList">每日收盤行情列表</param>
        /// <returns>成功數量</returns>
        public int InsertDailyQuoteList(List<DailyQuote> dailyQuoteList = null) {
            try {
                int affectedRowNumber = -1;

                //初始化CreateDateTime, UpdateDateTime
                dailyQuoteList = dailyQuoteList.Select(dq => {
                    dq.CreateDateTime = DateTime.Now;
                    dq.UpdateDateTime = DateTime.Now;
                    return dq;
                }).ToList();

                #region IF_NOT_EXISTS_THEN_INSERT_ALL

                string IF_NOT_EXISTS_THEN_INSERT_ALL =
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

                #endregion IF_NOT_EXISTS_THEN_INSERT_ALL

                OpenSqlConnection();
                affectedRowNumber = Execute(IF_NOT_EXISTS_THEN_INSERT_ALL, dailyQuoteList);
                return affectedRowNumber;
            } catch (Exception ex) {
                throw ex;
            }
        }

        /// <summary>
        /// Update每日收盤行情列表
        /// </summary>
        /// <param name="dailyQuoteList">每日收盤行情列表</param>
        /// <returns>成功數量</returns>
        public int SaveDailyQuoteList(List<DailyQuote> dailyQuoteList) {
            try {
                int affectedRowNumber = -1;
                //初始化CreateDateTime, UpdateDateTime
                dailyQuoteList = dailyQuoteList.Select(dq => {
                    dq.CreateDateTime = DateTime.Now;
                    dq.UpdateDateTime = DateTime.Now;
                    return dq;
                }).ToList();

                #region SAVE_ALL

                string SAVE_ALL =
                    @"IF EXISTS (SELECT TOP 1 * FROM DailyQuotes WHERE Date = @Date AND StockCode = @StockCode)
                      BEGIN
                        UPDATE DailyQuotes
                        SET
                          Date = @Date
                          , UpdateDateTime = @UpdateDateTime
                          , StockCode = @StockCode
                          , TradeVolume = @TradeVolume
                          , [Transaction] = @Transaction
                          , TradeValue = @TradeValue
                          , OpeningPrice = @OpeningPrice
                          , HighestPrice = @HighestPrice
                          , LowestPrice = @LowestPrice
                          , ClosingPrice = @ClosingPrice
                          , Direction = @Direction
                          , Change = @Change
                          , LastBestBidPrice = @LastBestBidPrice
                          , LastBestBidVolume = @LastBestBidVolume
                          , LastBestAskPrice = @LastBestAskPrice
                          , LastBestAskVolume = @LastBestAskVolume
                          , PriceEarningRatio = @PriceEarningRatio
                        WHERE Date = @Date
                        AND StockCode = @StockCode
                      END
                    ELSE
                    BEGIN
                      INSERT INTO DailyQuotes(
                        [Date]
                        , CreateDateTime
                        , StockCode
                        , TradeVolume
                        , [Transaction]
                        , TradeValue
                        , OpeningPrice
                        , HighestPrice
                        , LowestPrice
                        , ClosingPrice
                        , Direction
                        , Change
                        , LastBestBidPrice
                        , LastBestBidVolume
                        , LastBestAskPrice
                        , LastBestAskVolume
                        , PriceEarningRatio)
                      VALUES(
                        @Date
                        , @CreateDateTime
                        , @StockCode
                        , @TradeVolume
                        , @Transaction
                        , @TradeValue
                        , @OpeningPrice
                        , @HighestPrice
                        , @LowestPrice
                        , @ClosingPrice
                        , @Direction
                        , @Change
                        , @LastBestBidPrice
                        , @LastBestBidVolume
                        , @LastBestAskPrice
                        , @LastBestAskVolume
                        , @PriceEarningRatio)
                    END";

                #endregion SAVE_ALL

                OpenSqlConnection();
                affectedRowNumber = Execute(SAVE_ALL, dailyQuoteList);
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
            DailyQuote dailyQuote;

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