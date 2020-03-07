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
using Models.Google.API;
using Utility.Google.MapAPIs;

namespace Utility.MaskDataHandler
{
    public class MaskDataHandler
    {
        private static List<MaskData> maskDataList;
        public MaskDataHandler()
        {
            maskDataList = MaskDataSourceHandler.GetList();
        }
        public static List<MaskData> GetMaskDatasFromLocationSuffix(string locationSuffix)
        {
            if (maskDataList == null)
            {
                maskDataList = MaskDataSourceHandler.GetList();
            }

            List<MaskData> result = new List<MaskData>();

            int strLength = locationSuffix.Length;
            // var maskDataList = MaskDataSourceHandler.GetList();
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
            if (maskDataList == null)
            {
                maskDataList = MaskDataSourceHandler.GetList();
            }

            List<MaskData> result = new List<MaskData>();
            int _count = 0;

            int strLength = locationSuffix.Length;
            //var maskDataList = MaskDataSourceHandler.GetList();
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

        public static List<MaskData> GetTopMaskDatasByComputingDistance(string location, int count)
        {
            if (maskDataList == null)
            {
                maskDataList = MaskDataSourceHandler.GetList();
            }

            List<MaskData> result = new List<MaskData>();
            int _count = 0;
            

            var MaskDataDistancesList = new List<MaskDataDistances>();
            for (int i = 0; i < maskDataList.Count; i = i + 80)
            {
                StringBuilder destinationAddressBuilder = new StringBuilder();
                for (int j = i; j < i + 80 && j < maskDataList.Count; j++)
                {
                    destinationAddressBuilder.Append(maskDataList[j].Address);
                    destinationAddressBuilder.Append("|");
                }
                var destinationAddress = destinationAddressBuilder.Remove(destinationAddressBuilder.Length - 1, 1).ToString();
                var distanceMatrix = GeocodeHandler.GetDistanceMatrix(destinationAddress, location);
                for (int j = 0; j < distanceMatrix.rows.Count; j++)
                {
                    var row = distanceMatrix.rows[j];
                    // 距離(單位: 公尺)
                    int distance;
                    if(row.elements[0].status == "OK")
                    {
                        distance = row.elements[0].distance.value;
                    }
                    else
                    {
                        distance = Int32.MaxValue;
                    }

                    MaskDataDistancesList.Add(new MaskDataDistances{
                        maskDataIndex = i + j,
                        distance = distance
                    });
                }
                
                
            }
            MaskDataDistancesList.Sort(Utility.NumberUtil.Comparer.CompareDistance);
            foreach (var MaskDataDistances in  MaskDataDistancesList)
            {
                result.Add(maskDataList[MaskDataDistances.maskDataIndex]);
                _count ++;
                if(_count == count)
                {
                    break;
                }
            }
            return result;
        }
    }

    public class MaskDataDistances
    {
        public int maskDataIndex { get; set; }
        public int distance { get; set; }
    }
}