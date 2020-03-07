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

namespace Utility.StringUtil
{
    public class LocationHandler
    {
        public static string GetLocationSecondDivisionSuffix(string address)
        {
            int index = -1;
            if((index = address.IndexOf("區")) != -1)
            {
                return address.Substring(0, index);
            }
            if((index = address.IndexOf("鄉")) != -1)
            {
                return address.Substring(0, index);
            }
            if((index = address.IndexOf("鎮")) != -1)
            {
                return address.Substring(0, index);
            }
            if(address.IndexOf("縣") != -1 && (index = address.IndexOf("市")) != -1)
            {
                return address.Substring(0, index);
            }
            return "";
        }
    }
}
            