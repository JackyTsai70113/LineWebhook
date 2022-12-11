using Core.Domain.Entities.TWSE_Stock;
using DA.Repositories.Base;
using DA.Repositories.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.Repositories.TWSE_Stock {

    public class DividendDistributionRepository : BaseRepository<DividendDistribution>, IDividendDistributionRepository {

        /// <summary>
        /// 若資料庫中無對應的股利分派，則 Insert，否則 Update
        /// </summary>
        /// <param name="dividendDistributionList">股利分派列表</param>
        /// <returns>成功數量</returns>
        public int SaveDividendDistributionList(List<DividendDistribution> dividendDistributionList) {
            try {
                int affectedRowNumber = -1;

                //初始化CreateDateTime, UpdateDateTime
                dividendDistributionList = dividendDistributionList.Select(dd => {
                    dd.CreateDateTime = DateTime.Now;
                    dd.UpdateDateTime = DateTime.Now;
                    return dd;
                }).ToList();

                #region SAVE_ALL

                string SAVE_ALL =
                    @"IF EXISTS (SELECT TOP 1 * FROM DividendDistributions WHERE Year = @Year AND StockCode = @StockCode)
                      BEGIN
                        UPDATE DividendDistributions
                        SET
                          UpdateDateTime = @UpdateDateTime
                          , CashDividendsToBeDistributedFromRetainedEarnings = @CashDividendsToBeDistributedFromRetainedEarnings
                          , CashDividendsFromLegalReserveAndCapitalSurplus = @CashDividendsFromLegalReserveAndCapitalSurplus
                          , SharesDistributedFromEarnings = @SharesDistributedFromEarnings
                          , SharesDistributedFromLegalReserveAndCapitalSurplus = @SharesDistributedFromLegalReserveAndCapitalSurplus
                        WHERE Year = @Year
                        AND StockCode = @StockCode
                      END
                    ELSE
                      BEGIN
                        INSERT INTO DividendDistributions(
                          StockCode
                          , Year
                          , CreateDateTime
                          , UpdateDateTime
                          , CashDividendsToBeDistributedFromRetainedEarnings
                          , CashDividendsFromLegalReserveAndCapitalSurplus
                          , SharesDistributedFromEarnings
                          , SharesDistributedFromLegalReserveAndCapitalSurplus)
                        VALUES(
                          @StockCode
                          , @Year
                          , @CreateDateTime
                          , @UpdateDateTime
                          , @CashDividendsToBeDistributedFromRetainedEarnings
                          , @CashDividendsFromLegalReserveAndCapitalSurplus
                          , @SharesDistributedFromEarnings
                          , @SharesDistributedFromLegalReserveAndCapitalSurplus)
                      END";

                #endregion SAVE_ALL

                OpenSqlConnection();
                affectedRowNumber = Execute(SAVE_ALL, dividendDistributionList);
                return affectedRowNumber;
            } catch (Exception) {
                throw;
            }
        }
    }
}