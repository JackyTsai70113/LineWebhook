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
            var maskDataResponse = maskDealer.GetMaskDataResponse();
            var maskDataStrArr = maskDataResponse.Split("\r\n");

            // 取得 maskData 的 List
            var MaskDataList = new List<MaskData>();
            result += maskDataStrArr.Count();
            result += '\n';
            for(int i = 1; i < maskDataStrArr.Length - 1; i++)
            {
                var maskDataArr = maskDataStrArr[i].Split(',');

                if(!Int32.TryParse(maskDataArr[4], out int AdultMasks))
                {
                    var AdultMasksStr = maskDataArr[4];
                    AdultMasks = 99999999;
                }

                if(!Int32.TryParse(maskDataArr[5], out int ChildMasks))
                {
                    var ChildMasksStr = maskDataArr[5];
                    ChildMasks = 99999999;
                }

                if(!DateTime.TryParse(maskDataArr[6], out DateTime UpdateTime))
                {
                    var UpdateTimeStr = maskDataArr[6];
                    UpdateTime = DateTime.MinValue;
                }

                MaskDataList.Add(new MaskData{
                    Id = maskDataArr[0],
                    Name = maskDataArr[1],
                    Address = maskDataArr[2],
                    PhoneNumber = maskDataArr[3],
                    AdultMasks = AdultMasks,
                    ChildMasks = ChildMasks,
                    UpdateTime = UpdateTime
                });
            }
            ViewData["result"] = "MaskDataList.Count: " + MaskDataList.Count.ToString();
            return View();
        }

        public IActionResult List()
        {
            MaskDealer maskDealer = new MaskDealer();
            var maskDataResponse = maskDealer.GetMaskDataResponse();
            var maskDataStrArr = maskDataResponse.Split("\r\n");
            
            // 取得 maskData 的 List
            var MaskDataList = new List<MaskData>();
            for(int i = 1; i < maskDataStrArr.Length - 1; i++)
            {
                var maskDataArr = maskDataStrArr[i].Split(',');
                MaskDataList.Add(new MaskData{
                    Id = maskDataArr[0],
                    Name = maskDataArr[1],
                    Address = maskDataArr[2],
                    PhoneNumber = maskDataArr[3],
                    AdultMasks = Int32.TryParse(maskDataArr[4], out int AdultMasksNum) ? AdultMasksNum : 0,
                    ChildMasks = Int32.TryParse(maskDataArr[5], out int ChildMasksNum) ? ChildMasksNum : 0,
                    UpdateTime = DateTime.TryParse(maskDataArr[6], out DateTime updateTime) ? updateTime : DateTime.MinValue
                });
            }
            var li1 = new List<int>();
            CheckMaskData(MaskDataList);
            ViewData["maskData"] = maskDataResponse;
            return View(MaskDataList);
        }

        private void CheckMaskData(List<MaskData> maskDataList)
        {
            /*
                
                市 City
                    區 District
                縣 County
                    鎮/鄉 Township
                
            */
            
            //Dictionary<string, int> checkFirstDict;
            //Dictionary<string, int> checkSecondDict;
            foreach (var maskData in maskDataList)
            {
                var CityIndex = maskData.Address.IndexOf("市");
                var CountyIndex = maskData.Address.IndexOf("縣");
                var s = maskData.Address.Substring(6);

            }
        }

        public IActionResult Create()
        {
            MaskDealer maskDealer = new MaskDealer();
            var maskDataStringList = maskDealer.GetMaskDataResponse().Split('\n')[1];
            var maskDataArr = maskDataStringList.Split(',');
            return View();
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
