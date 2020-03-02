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

        public string GetMaskData()
        {
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var httpreq =
                (HttpWebRequest)WebRequest.Create(new Uri(uri));

            var response = httpreq.GetResponse();
            Stream recvStream = response.GetResponseStream();
            //recvStream.ReadTimeout = 20000;
            byte[] buffer = new byte[819];

            int size = 0;
            int total = 0;
            do
            {
                size = recvStream.Read(buffer, total, buffer.Length - total);
                if (size > 0)
                {
                    total += size;
                }
            }
            while (size > 0);

            string retStr = Encoding.UTF8.GetString(buffer, 0, total);
            return retStr;

            // return Enumerable.Range(1, 5).Select(index => index.ToString())
            //     .ToArray();
        }
    }
}
