using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using DA.Managers.TWSE_Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Website.Controllers {

    public class StockController : Controller {

        public IActionResult Index() {
            Array values = Enum.GetValues(typeof(StockCodeEnum));
            List<DividendDistribution> dividendDistributions = CashDividendManager.Get(values);
            return Ok(dividendDistributions);
        }
    }
}