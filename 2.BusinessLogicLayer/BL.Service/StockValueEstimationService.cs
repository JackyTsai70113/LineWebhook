using System.Text.Json;
using BL.Service.Interface.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using DA.Managers.Interfaces.TWSE_Stock;
using DA.Managers.TWSE_Stock;
using Microsoft.Extensions.Logging;

namespace BL.Service
{
    public class StockValueEstimationService : IStockValueEstimationService
    {
        private readonly ILogger<StockValueEstimationService> _logger;
        private readonly IDividendDistributionManager _dividendDistributionManager;
        private readonly IYearlyTradingInformationManager _yearlyTradingInformationManager;

        public StockValueEstimationService(ILogger<StockValueEstimationService> logger)
        {
            _logger = logger;
            _yearlyTradingInformationManager = new YearlyTradingInformationManager();
            _dividendDistributionManager = new DividendDistributionManager();
        }

        /// <summary>
        /// 根據 股票代號 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnum">股票代號</param>
        /// <returns>儲存數量</returns>
        public int CrawlForStockValueEstimationIn10YearsAndSave(StockCodeEnum stockCodeEnum)
        {
            int successNumber = 0;

            //根據 股票代號 抓取股利分派列表
            List<DividendDistribution> dividendDistributionList = _dividendDistributionManager.CrawlDividendDistribution(stockCodeEnum);
            _logger.LogInformation("dividendDistributionList:{dividendDistributionList}", JsonSerializer.Serialize(dividendDistributionList));

            //根據 年度交易資訊 抓取股利分派列表
            List<YearlyTradingInformation> yearlyTradingInformationList = _yearlyTradingInformationManager.CrawlYearlyTradingInformation(stockCodeEnum);
            _logger.LogInformation("yearlyTradingInformationList:{yearlyTradingInformationList}", JsonSerializer.Serialize(yearlyTradingInformationList));


            StockValueEstimation stockValueEstimation = GetStockValueEstimationList(dividendDistributionList, yearlyTradingInformationList);
            _logger.LogInformation("stockValueEstimation:{stockValueEstimation}", JsonSerializer.Serialize(stockValueEstimation));
            return successNumber;
        }

        /// <summary>
        /// 根據 股票代號列表 抓取股利分派後計算，並分別更新DB: DividendDistribution, YearlyTradingInformation, StockValueEstimation。
        /// </summary>
        /// <param name="stockCodeEnums">股票代號列表</param>
        /// <returns>儲存數量</returns>
        public int CrawlForStockValueEstimationIn10YearsAndSave(IEnumerable<StockCodeEnum> stockCodeEnums)
        {
            int successNumber = 0;
            foreach (StockCodeEnum stockCodeEnum in stockCodeEnums)
            {
                successNumber += CrawlForStockValueEstimationIn10YearsAndSave(stockCodeEnum);
            }

            return successNumber;
        }

        protected static StockValueEstimation GetStockValueEstimationList(List<DividendDistribution> dividendDistributionList, List<YearlyTradingInformation> yearlyTradingInformationList)
        {
            StockValueEstimation stockValueEstimation = new();

            //先排序
            IOrderedEnumerable<DividendDistribution> DividendDistributionEnumerable = dividendDistributionList.OrderByDescending(x => x.Year);
            IOrderedEnumerable<YearlyTradingInformation> YearlyTradingInformationEnumerable =
                yearlyTradingInformationList.OrderByDescending(x => x.Year);

            stockValueEstimation.StockCode = dividendDistributionList.First().StockCode;

            #region 股利法

            stockValueEstimation.RecentDividends = dividendDistributionList.First().Dividends;
            stockValueEstimation.DividendsIn5Years = dividendDistributionList.Take(5).Average(x => x.Dividends);
            stockValueEstimation.DividendsIn10Years = dividendDistributionList.Take(10).Average(x => x.Dividends);

            #endregion 股利法

            #region 股價法

            stockValueEstimation.CheapPriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.LowestPrice);
            stockValueEstimation.ReasonablePriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.AverageClosingPrice);
            stockValueEstimation.ExpensivePriceByStockPriceOver10Years = yearlyTradingInformationList.Average(x => x.HighestPrice);

            #endregion 股價法

            return stockValueEstimation;
        }
    }
}