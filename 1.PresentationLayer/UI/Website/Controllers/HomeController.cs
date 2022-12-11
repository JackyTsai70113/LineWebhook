using System;
using System.Collections.Generic;
using System.Diagnostics;
using BL.Services.Interfaces;
using BL.Services.YahooFinance;
using Microsoft.AspNetCore.Mvc;
using Website.Models;
using Core.Domain.DTO;
using Microsoft.Extensions.Configuration;

namespace Website.Controllers {

    public class HomeController : Controller {
        private readonly IConfigurationRoot _configRoot;
        private readonly IMaskInstitutionService _maskInstitutionService;

        public HomeController(IConfiguration config, IMaskInstitutionService maskInstitutionService) {
            _configRoot = (IConfigurationRoot)config;
            _maskInstitutionService = maskInstitutionService;
        }

        public IActionResult Test() {
            string s = $"網站正常運作中，時間: {DateTime.UtcNow.AddHours(8)}";
            return Content(s);
        }

        public IActionResult Index() {
            int maskInstitutionCount = _maskInstitutionService.GetMaskInstitutionCount();
            ViewData["result"] = "MaskDateList.Count: " + maskInstitutionCount;
            return View();
        }

        public IActionResult List() {
            string address = "110台灣台北市信義區虎林街132巷37號";
            List<MaskInstitution> maskDataList =
                _maskInstitutionService.GetMaskInstitutions(address);

            return View(maskDataList);
        }

        public IActionResult Create() {
            return View();
        }

        public IActionResult Privacy() {
            return View();
        }

        public IActionResult KD() {
            StockService stockService = new StockService();
            List<object[]> subCandles = stockService.GetSubCandles();

            return View(subCandles);
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error() {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}