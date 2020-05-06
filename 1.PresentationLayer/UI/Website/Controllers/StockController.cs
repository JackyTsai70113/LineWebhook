using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BL.Services;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using Core.Domain.Interafaces.Services;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {

    public class StockController : Controller {

        public StockController() {
            DailyQuoteService = new DailyQuoteService();
        }

        //public DailyQuoteManager MyProperty { get; set; }
        public IDailyQuoteService DailyQuoteService { get; set; }

        public IActionResult Index() {
            //DailyQuoteService d = new DailyQuoteService();
            //d.CreateDailyQuote();
            //Array values = Enum.GetValues(typeof(StockCodeEnum));
            //List<DividendDistribution> dividendDistributions = CashDividendManager.Get(values);
            //return Ok(dividendDistributions);
            int successDailyQuotes = DailyQuoteService.GetDailyQuoteListAndSave(2020, StockCategoryEnum.FinancialAndInsurance);
            return Ok(successDailyQuotes);
        }

        public IActionResult CreateDailyQuote() {
            return Ok("");
        }

        public IActionResult GetDailyQuoteList() {
            return Ok("");
        }
    }
}