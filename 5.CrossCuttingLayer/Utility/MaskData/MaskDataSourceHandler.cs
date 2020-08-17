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

namespace Utility.MaskDatas {

    public class MaskDataSourceHandler {

        public static List<MaskData> GetList() {
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var request = (HttpWebRequest)WebRequest.Create(uri);

            var response = request.GetResponse();
            var stream = response.GetResponseStream();
            var streamReader = new StreamReader(stream);

            string maskDataStr = streamReader.ReadToEnd();

            var maskDataStrArr = maskDataStr.Split('\n');

            var maskDataList = new List<MaskData>();
            for (int i = 1; i < maskDataStrArr.Length - 1; i++) {
                maskDataList.Add(new MaskData(maskDataStrArr[i]));
            }
            return maskDataList;
        }

        public static void MaskDataAddressStatistics() {
            List<MaskData> maskDatas = GetList();
            List<string> strings1 = new List<string>();
            List<string> strings2 = new List<string>();
            List<string> strings3 = new List<string>();
            List<string> strings4 = new List<string>();
            List<string> strings5 = new List<string>();
            foreach (var maskData in maskDatas) {
                if (maskData.Address.Contains("¸ô") && maskData.Address.Contains("µó")) {
                    strings1.Add(maskData.Address);
                } else if (maskData.Address.Contains("¸ô")) {
                    strings2.Add(maskData.Address);
                } else if (maskData.Address.Contains("µó")) {
                    strings3.Add(maskData.Address);
                } else {
                    strings4.Add(maskData.Address);
                }
            }
        }
    }
}