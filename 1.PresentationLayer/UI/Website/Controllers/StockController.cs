using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using BL.Services;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Enums;
using DA.Managers.TWSE_Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Website.Controllers {

    public class StockController : Controller {

        public IActionResult Index() {
            DailyQuoteService d = new DailyQuoteService();
            d.CreateDailyQuote();
            Array values = Enum.GetValues(typeof(StockCodeEnum));
            List<DividendDistribution> dividendDistributions = CashDividendManager.Get(values);
            return Ok(dividendDistributions);
        }

        public IActionResult CreateDailyQuote() {
            //Array values = Enum.GetValues(typeof(StockCodeEnum));
            DailyQuote dividendDistributions = DailyQuoteManager.GetDailyQuote();

            return Ok(dividendDistributions);
        }
    }
}