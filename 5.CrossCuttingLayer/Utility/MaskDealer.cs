using System;
using System.IO;
using System.Net;
using System.Text;

namespace Utility
{

    public class MaskDealer
    {
        public string MaskStr()
        {
            return "HI";
        }

        public string GetMaskDataResponse()
        {
            string result;
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var httpreq =
                (HttpWebRequest)WebRequest.Create(new Uri(uri));

            using(var response = httpreq.GetResponse())
            using(Stream stream = response.GetResponseStream())
            using(StreamReader streamReader = new StreamReader(stream))
            {
                result = streamReader.ReadToEnd();
            }
            return result;
        }
    }
}
