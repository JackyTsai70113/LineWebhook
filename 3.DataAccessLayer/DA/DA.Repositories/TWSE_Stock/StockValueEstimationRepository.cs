using Core.Domain.Entities.TWSE_Stock;
using DA.Repositories.Base;
using DA.Repositories.Interfaces.TWSE_Stock;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DA.Repositories.TWSE_Stock {

    public class StockValueEstimationRepository : BaseRepository<StockValueEstimation>, IStockValueEstimationRepository {

        /// <summary>
        /// 若資料庫中無對應則 Insert，否則 Update
        /// </summary>
        /// <param name="StockValueEstimation">股票價值估算</param>
        /// <returns>成功數量</returns>
        public int SaveStockValueEstimation(StockValueEstimation stockValueEstimation) {
            try {
                int affectedRowNumber = -1;

                //初始化CreateDateTime, UpdateDateTime
                stockValueEstimation.CreateDateTime = DateTime.Now;
                stockValueEstimation.UpdateDateTime = DateTime.Now;

                #region SAVE_ALL

                string SAVE_ALL =
                    @"IF EXISTS (SELECT TOP 1 * FROM StockValueEstimations WHERE StockCode = @StockCode)
                        BEGIN
                        UPDATE StockValueEstimations
                        SET
                            UpdateDateTime = @UpdateDateTime

                            , RecentDividends = @RecentDividends
                            , DividendsIn5Years = @DividendsIn5Years
                            , DividendsIn10Years = @DividendsIn10Years

                            , CheapPriceByRecentDividends = @CheapPriceByRecentDividends
                            , ReasonablePriceByRecentDividends = @ReasonablePriceByRecentDividends
                            , ExpensivePriceByRecentDividends = @ExpensivePriceByRecentDividends

                            , CheapPriceByDividendsIn5Years = @CheapPriceByDividendsIn5Years
                            , ReasonablePriceByDividendsIn5Years = @ReasonablePriceByDividendsIn5Years
                            , ExpensivePriceByDividendsIn5Years = @ExpensivePriceByDividendsIn5Years

                            , CheapPriceByDividendsIn10Years = @CheapPriceByDividendsIn10Years
                            , ReasonablePriceByDividendsIn10Years = @ReasonablePriceByDividendsIn10Years
                            , ExpensivePriceByDividendsIn10Years = @ExpensivePriceByDividendsIn10Years

                            , CheapPriceByStockPriceOver10Years = @CheapPriceByStockPriceOver10Years
                            , ReasonablePriceByStockPriceOver10Years = @ReasonablePriceByStockPriceOver10Years
                            , ExpensivePriceByStockPriceOver10Years = @ExpensivePriceByStockPriceOver10Years
                        WHERE StockCode = @StockCode
                        END
                    ELSE
                        BEGIN
                        INSERT INTO StockValueEstimations(
                            StockCode
                            , CreateDateTime
                            , UpdateDateTime

                            , RecentDividends
                            , DividendsIn5Years
                            , DividendsIn10Years

                            , CheapPriceByRecentDividends
                            , ReasonablePriceByRecentDividends
                            , ExpensivePriceByRecentDividends

                            , CheapPriceByDividendsIn5Years
                            , ReasonablePriceByDividendsIn5Years
                            , ExpensivePriceByDividendsIn5Years

                            , CheapPriceByDividendsIn10Years
                            , ReasonablePriceByDividendsIn10Years
                            , ExpensivePriceByDividendsIn10Years

                            , CheapPriceByStockPriceOver10Years
                            , ReasonablePriceByStockPriceOver10Years
                            , ExpensivePriceByStockPriceOver10Years
                        )
                        VALUES(
                            @StockCode
                            , @CreateDateTime
                            , @UpdateDateTime

                            , @RecentDividends
                            , @DividendsIn5Years
                            , @DividendsIn10Years

                            , @CheapPriceByRecentDividends
                            , @ReasonablePriceByRecentDividends
                            , @ExpensivePriceByRecentDividends

                            , @CheapPriceByDividendsIn5Years
                            , @ReasonablePriceByDividendsIn5Years
                            , @ExpensivePriceByDividendsIn5Years

                            , @CheapPriceByDividendsIn10Years
                            , @ReasonablePriceByDividendsIn10Years
                            , @ExpensivePriceByDividendsIn10Years

                            , @CheapPriceByStockPriceOver10Years
                            , @ReasonablePriceByStockPriceOver10Years
                            , @ExpensivePriceByStockPriceOver10Years
                        )
                        END";

                #endregion SAVE_ALL

                OpenSqlConnection();
                affectedRowNumber = Execute(SAVE_ALL, stockValueEstimation);
                return affectedRowNumber;
            } catch (Exception) {
                throw;
            }
        }
    }
}