using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using BL.Services;
using BL.Services.Interfaces;
using BL.Services.YahooFinance;
using Core.Domain.Utilities;
using Microsoft.AspNetCore.Mvc;
using Website.Models;

namespace Website.Controllers {

    public class HomeController : Controller {
        private readonly IMaskInstitutionService _maskInstitutionService;

        public HomeController(IMaskInstitutionService maskInstitutionService) {
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
            // 取得 maskData 的 List
            var maskDataList = _maskInstitutionService.GetMaskInstitutionsByComputingDistance("110台灣台北市信義區虎林街132巷37號");
            StringBuilder builder = new StringBuilder();
            foreach (var maskData in maskDataList) {
                builder.AppendLine($"{maskData.Name}: 成人({maskData.numberOfAdultMasks})/兒童({maskData.numberOfChildMasks})");
            }
            ViewData["testMaskData"] = builder.ToString();
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