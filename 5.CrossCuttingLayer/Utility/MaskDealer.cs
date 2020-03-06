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
            return "HI1";
        }

        public string GetMaskDataResponse()
        {
            string result;
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var httpreq =
                (HttpWebRequest)WebRequest.Create(new Uri(uri));

            var response = httpreq.GetResponse();
            Stream stream = response.GetResponseStream();
            StreamReader streamReader = new StreamReader(stream);

            byte[] buffer = new byte[168888];
            int readSize = 0, offset = 0;
            do
            {
                readSize = stream.Read(buffer, offset, buffer.Length - offset);
                offset += readSize;
            }while(readSize > 0);
            //result = streamReader.ReadToEnd();
            result = Encoding.UTF8.GetString(buffer, 0, offset);
            return result;
        }
    }
}
