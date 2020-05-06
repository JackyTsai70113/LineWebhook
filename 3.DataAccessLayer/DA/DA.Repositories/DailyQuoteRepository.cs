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
                using (SqlConnection sqlConnection = SqlConnection) {
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
                using (SqlConnection sqlConnection = SqlConnection) {
                    sqlConnection.Open();
                    affectedRowNumber = sqlConnection.Execute(IF_NOT_EXISTS_THEN_INSERT_ALL, dailyQuoteList);
                    return affectedRowNumber;
                }
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}