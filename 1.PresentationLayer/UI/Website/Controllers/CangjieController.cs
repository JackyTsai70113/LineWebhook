using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

namespace Website.Controllers {

    /// <summary>
    /// 倉頡相關控制器
    /// </summary>
    public class CangjieController : Controller {

        public IActionResult Index() {
            return View();
        }
    }
}