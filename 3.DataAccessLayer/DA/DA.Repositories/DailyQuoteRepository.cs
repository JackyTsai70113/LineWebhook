using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Interafaces.Repositories;
using Core.Domain.Utilities;
using DA.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Text;
using Dapper;

namespace DA.Repositories {

    public class DailyQuoteRepository : BaseRepository, IDailyQuoteRepository {

        public void InsertDailyQuote(DailyQuote dailyQuote = null) {
            using (SqlConnection sqlConnection = SqlConnection) {
                sqlConnection.Open();

                string sql = $"INSERT INTO " +
                    $"[dbo].[DailyQuotes]([Date],[CreateDateTime],[StockCode],[TradeVolume],[Transaction],[TradeValue],[OpeningPrice],[HighestPrice],[LowestPrice],[ClosingPrice],[Direction],[Change],[LastBestBidPrice],[LastBestBidVolume],[LastBestAskPrice],[LastBestAskVolume],[PriceEarningRatio])" +
                    $"VALUES('2020/5/4', '2020/5/5', 12999, 0, 0, 0, 0.0, 0.0, 0.0, 0.0, 0, 0.0, 0.0, 0, 0.0, 0, 0.0)";
                var identity = sqlConnection.Execute(sql);
            }
        }
    }
}