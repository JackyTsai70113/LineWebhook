using Core.Domain.Entities.TWSE_Stock;

namespace DA.Repositories.Interfaces.TWSE_Stock {

    public interface IStockValueEstimationRepository : IBaseRepository {

        /// <summary>
        /// 若資料庫中無對應則 Insert，否則 Update
        /// </summary>
        /// <param name="StockValueEstimation">股票價值估算</param>
        /// <returns>成功數量</returns>
        int SaveStockValueEstimation(StockValueEstimation stockValueEstimation);
    }
}