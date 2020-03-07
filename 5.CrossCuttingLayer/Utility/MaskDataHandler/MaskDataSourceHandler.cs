using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Utility;
using Newtonsoft.Json;
using System.Net;
using System.Text;
using Models.MaskDatas;

namespace Utility.MaskDataHandler
{
    public class MaskDataSourceHandler
    {
        // public static string GetDataStr()
        // {
            
        // }

        public static List<MaskData> GetList()
        {
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv"; 
            var request = (HttpWebRequest)WebRequest.Create(uri); 

            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream);

            string maskDataStr = streamReader.ReadToEnd();

            var maskDataStrArr = maskDataStr.Split('\n');
            
            var maskDataList = new List<MaskData>();
            for(int i = 1; i < maskDataStrArr.Length - 1; i++)
            {
                var maskDataArr = maskDataStrArr[i].Split(',');
                if (!Int32.TryParse(maskDataArr[4], out int adultMasks))
                {
                    Console.WriteLine($"Ex: Cannot parse {maskDataArr[4]} to Int.");
                    adultMasks = Int32.MaxValue;
                }
                if (!Int32.TryParse(maskDataArr[5], out int childMasks))
                {
                    Console.WriteLine($"Ex: Cannot parse {maskDataArr[5]} to Int.");
                    childMasks = Int32.MaxValue;
                }
                if (!DateTime.TryParse(maskDataArr[6], out DateTime updateTime))
                {
                    Console.WriteLine($"Ex: Cannot parse {maskDataArr[6]} to Int.");
                    updateTime = DateTime.MinValue;
                }
                maskDataList.Add(new MaskData
                {
                    Id = maskDataArr[0],
                    Name = maskDataArr[1],
                    Address = maskDataArr[2],
                    PhoneNumber = maskDataArr[3],
                    AdultMasks = adultMasks,
                    ChildMasks = childMasks,
                    UpdateTime = updateTime
                });
            }
            return maskDataList;
        }
    }
}
