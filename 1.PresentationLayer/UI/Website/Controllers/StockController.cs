using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using DA.Managers.TWSE_Stock;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Website.Controllers {

    public class StockController : Controller {

        public IActionResult Index() {
            var a = ExchangeManager.Get2884();
            return Ok(a);
        }
    }
}