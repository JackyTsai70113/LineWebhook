using Core.Domain.Enums;

namespace BL.Service.Interface.TWSE_Stock
{
    public interface IStockValueEstimationService
    {
        /// <summary>
        /// 根據 股票代號 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnum">股票代號</param>
        /// <returns>儲存數量</returns>
        int CrawlForStockValueEstimationIn10YearsAndSave(StockCodeEnum stockCodeEnum);

        /// <summary>
        /// 根據 股票代號列表 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnums">股票代號列表</param>
        /// <returns>儲存數量</returns>
        int CrawlForStockValueEstimationIn10YearsAndSave(IEnumerable<StockCodeEnum> stockCodeEnums);
    }
}