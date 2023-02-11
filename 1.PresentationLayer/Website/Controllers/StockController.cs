using System;
using System.Collections.Generic;
using System.Linq;
using BL.Service.Interface.TWSE_Stock;
using BL.Service.Stock;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using YahooQuotesApi;

namespace Website.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class StockController : ControllerBase
    {
        private readonly IDailyQuoteService DailyQuoteService;
        private readonly IDividendDistributionService DividendDistributionService;
        private readonly IStockValueEstimationService StockValueEstimationService;

        public StockController(IDailyQuoteService dailyQuoteService, IDividendDistributionService dividendDistributionService, IStockValueEstimationService stockValueEstimationService)
        {
            DailyQuoteService = dailyQuoteService;
            DividendDistributionService = dividendDistributionService;
            StockValueEstimationService = stockValueEstimationService;
        }

        [HttpGet]
        [Route("")]
        public IActionResult Index()
        {
            IActionResult actionResult = CrawlForStockValueEstimationIn10Years();
            return actionResult;
        }

        #region 每日報表 DailyQuote

        /// <summary>
        /// 資料有誤時可指定這隻api進行資料的更新
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("quote/day")]
        public IActionResult CrawlDailyQuoteList()
        {
            DailyQuoteService.CrawlDailyQuoteListByDate(new DateTime(2020, 08, 26), StockCategoryEnum.FinancialAndInsurance);
            return Ok();
        }

        [HttpGet]
        [Route("quote/month")]
        public IActionResult CrawlDailyQuoteListByMonth(string dateStr = null)
        {
            DateTime date = DateTime.Now;
            if (dateStr != null)
            {
                if (!dateStr.TryParse(out date))
                {
                    throw new Exception("字串格式錯誤，無法轉型為日期.");
                }
            }

            DailyQuoteService.CrawlDailyQuoteListByMonth(date, StockCategoryEnum.FinancialAndInsurance);
            return Ok();
        }

        [HttpGet]
        [Route("quote/year")]
        public IActionResult CrawlDailyQuoteListAndInsertByYear()
        {
            DailyQuoteService.CrawlDailyQuoteListByYear(2010, StockCategoryEnum.FinancialAndInsurance);
            return Ok();
        }

        #endregion 每日報表 DailyQuote

        #region 股利分派 DividendDistribution

        [HttpGet]
        [Route("day")]
        public IActionResult GetDividendDistributionIn10Years()
        {
            List<DividendDistribution> dividendDistributionList = DividendDistributionService.CrawlDividendDistributionListByStockCodeEnumArray();
            return Ok(dividendDistributionList);
        }

        #endregion 股利分派 DividendDistribution

        #region 股票價值估算 StockValueEstimation

        [HttpGet]
        [Route("stock_value")]
        public IActionResult CrawlForStockValueEstimationIn10Years()
        {
            List<StockCodeEnum> stockCodeEnums = Enum.GetValues(typeof(StockCodeEnum)).Cast<StockCodeEnum>().ToList();
            var result = StockValueEstimationService.CrawlForStockValueEstimationIn10YearsAndSave(
                stockCodeEnums.GetRange(0, 6));
            return Ok(result);
        }

        #endregion 股票價值估算 StockValueEstimation


        [HttpGet]
        [Route("price_ticks")]
        public ActionResult<PriceTick[]> KD()
        {
            var priceTicks = StockService.GetPriceTicks();

            return Ok(priceTicks);
        }
    }
}