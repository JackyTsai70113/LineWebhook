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
    public class MaskDataHandler
    {
        public static List<MaskData> GetMaskDatasFromLocationSuffix(string locationSuffix)
        {
            List<MaskData> result = new List<MaskData>();

            int strLength = locationSuffix.Length;
            var maskDataList = MaskDataSourceHandler.GetList();
            foreach(var maskData in  maskDataList)
            {
                if (maskData.Address.Substring(0, strLength) == locationSuffix)
                {
                    result.Add(maskData);
                }
            }
            return result;
        } 

        public static List<MaskData> GetTopMaskDatasFromLocationSuffix(string locationSuffix, int count)
        {
            List<MaskData> result = new List<MaskData>();
            int _count = 0;

            int strLength = locationSuffix.Length;
            var maskDataList = MaskDataSourceHandler.GetList();
            foreach(var maskData in  maskDataList)
            {
                if (maskData.Address.Substring(0, strLength) == locationSuffix)
                {
                    result.Add(maskData);
                    _count ++;
                    if(_count == count)
                    {
                        break;
                    }
                }
            }
            return result;
        } 
    }
}