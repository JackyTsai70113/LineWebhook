using Core.Domain.Entities.TWSE_Stock.Exchange;
using DA.Repositories.Base;
using DA.Repositories.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.Repositories.TWSE_Stock {

    public class YearlyTradingInformationRepository : BaseRepository<YearlyTradingInformation>, IYearlyTradingInformationRepository {

        /// <summary>
        /// 若資料庫中無對應的年度交易資訊，則 Insert，否則 Update
        /// </summary>
        /// <param name="yearlyTradingInformationList">年度交易資訊列表</param>
        /// <returns>成功數量</returns>
        public int SaveYearlyTradingInformationList(List<YearlyTradingInformation> yearlyTradingInformationList) {
            try {
                int affectedRowNumber = -1;

                //初始化CreateDateTime, UpdateDateTime
                yearlyTradingInformationList = yearlyTradingInformationList.Select(x => {
                    x.CreateDateTime = DateTime.Now;
                    x.UpdateDateTime = DateTime.Now;
                    return x;
                }).ToList();

                #region SAVE_ALL

                string SAVE_ALL =
                    @"IF EXISTS (SELECT TOP 1 Year, StockCode FROM YearlyTradingInformations WHERE Year = @Year AND StockCode = @StockCode)
                      BEGIN
                        UPDATE YearlyTradingInformations
                        SET
                          UpdateDateTime = @UpdateDateTime
                          , TradeVolume = @TradeVolume
                          , TradeValue = @TradeValue
                          , [Transaction] = @Transaction
                          , HighestPrice = @HighestPrice
                          , HighestPriceDate = @HighestPriceDate
                          , LowestPrice = @LowestPrice
                          , LowestPriceDate = @LowestPriceDate
                          , AverageClosingPrice = @AverageClosingPrice
                        WHERE Year = @Year
                        AND StockCode = @StockCode
                      END
                    ELSE
                      BEGIN
                        INSERT INTO YearlyTradingInformations(
                          Year
                          , StockCode
                          , CreateDateTime
                          , UpdateDateTime
                          , TradeVolume
                          , TradeValue
                          , [Transaction]
                          , HighestPrice
                          , HighestPriceDate
                          , LowestPrice
                          , LowestPriceDate
                          , AverageClosingPrice)
                        VALUES(
                          @Year
                          , @StockCode
                          , @CreateDateTime
                          , @UpdateDateTime
                          , @TradeVolume
                          , @TradeValue
                          , @Transaction
                          , @HighestPrice
                          , @HighestPriceDate
                          , @LowestPrice
                          , @LowestPriceDate
                          , @AverageClosingPrice)
                      END";

                #endregion SAVE_ALL

                OpenSqlConnection();
                affectedRowNumber = Execute(SAVE_ALL, yearlyTradingInformationList);
                return affectedRowNumber;
            } catch (Exception ex) {
                throw ex;
            }
        }
    }
}