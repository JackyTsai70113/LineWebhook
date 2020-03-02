using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Website.Models;
using Utility;

namespace Website.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            string result = "";
            MaskDealer maskDealer = new MaskDealer();
            var maskDataStringList = maskDealer.GetMaskData().Split('\n');
            result += maskDataStringList.Count();
            result += '\n';
            foreach(var maskdataStr in maskDataStringList)
            {
                string[] maskdataList = maskdataStr.Split(',');
                result += maskdataList.Count();
                result += '\n';
                result += maskdataList[0];
                result += '\n';
                result += maskdataList[1];
                result += '\n';
            }
            ViewData["test"] = "test123";
            ViewData["result"] = result;
            return View();
        }

        public IActionResult Create()
        {
            MaskDealer maskDealer = new MaskDealer();
            var maskDataStringList = maskDealer.GetMaskData().Split('\n')[1];
            var maskDataArr = maskDataStringList.Split(',');
            // DateTime oDate = 
            // DateTime.ParseExact(iString, "yyyy-MM-dd HH:mm tt",null);
            var data = new InstitutionMaskData
            {
                //Id = (int)maskDataArr[0],
                Name = maskDataArr[1],
                Address = maskDataArr[2],
                PhoneNumber = maskDataArr[3],
                //AdultMasks = int.Parse(maskDataArr[4]),
                //ChildMasks = int.Parse(maskDataArr[5]),
                UpdateTime = maskDataArr[6]


            };
            return View(data);
        }

        public IActionResult WriteResponseWithReturn()
        {
            Response.ContentType = "text/html";

            using (StreamWriter sw = new StreamWriter(Response.Body))
            {
                sw.Write("Write a string to response in WriteResponseWithReturn!");
            }

            return new EmptyResult();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
