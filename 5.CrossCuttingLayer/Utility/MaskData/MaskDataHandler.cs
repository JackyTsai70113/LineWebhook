using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Models.Google.API;
using Models.MaskDatas;
using Newtonsoft.Json;
using Utility;
using Utility.Google.MapAPIs;
using Utility.StringUtil;

namespace Utility.MaskDatas {

    public class MaskDataHandler {
        private static List<MaskData> maskDataList;

        public MaskDataHandler() {
            maskDataList = MaskDataSourceHandler.GetList();
        }

        public static List<MaskData> GetMaskDatasFromLocationSuffix(string locationSuffix) {
            if (maskDataList == null) {
                maskDataList = MaskDataSourceHandler.GetList();
            }

            List<MaskData> result = new List<MaskData>();

            int strLength = locationSuffix.Length;
            // var maskDataList = MaskDataSourceHandler.GetList();
            foreach (var maskData in maskDataList) {
                if (maskData.Address.Substring(0, strLength) == locationSuffix) {
                    result.Add(maskData);
                }
            }
            return result;
        }

        public static List<MaskData> GetTopMaskDatasFromLocationSuffix(string locationSuffix, int count = Int32.MaxValue) {
            if (maskDataList == null) {
                maskDataList = MaskDataSourceHandler.GetList();
            }

            List<MaskData> result = new List<MaskData>();
            int _count = 0;

            int strLength = locationSuffix.Length;
            foreach (var maskData in maskDataList) {
                if (maskData.Address.Substring(0, strLength) == locationSuffix) {
                    result.Add(maskData);
                    _count++;
                    if (_count == count) {
                        break;
                    }
                }
                string tempLocationSuffix = locationSuffix;
                if (tempLocationSuffix.Contains("臺")) {
                    tempLocationSuffix = tempLocationSuffix.Replace("臺", "台");
                    if (maskData.Address.Substring(0, strLength) == tempLocationSuffix) {
                        result.Add(maskData);
                        _count++;
                        if (_count == count) {
                            break;
                        }
                    }
                } else if (tempLocationSuffix.Contains("台")) {
                    tempLocationSuffix = tempLocationSuffix.Replace("台", "臺");
                    if (maskData.Address.Substring(0, strLength) == tempLocationSuffix) {
                        result.Add(maskData);
                        _count++;
                        if (_count == count) {
                            break;
                        }
                    }
                }
            }
            return result;
        }

        public static List<MaskData> GetTopMaskDatasByComputingDistance(string location, int count = Int32.MaxValue) {
            string SecondDivision = LocationHandler.GetLocationSecondDivision(location);
            List<MaskData> maskDataList = GetTopMaskDatasFromLocationSuffix(SecondDivision, 5);
            return maskDataList.Take(5).ToList();
        }
    }

    public class MaskDataDistances {
        public int maskDataIndex { get; set; }
        public int distance { get; set; }
    }

    public class Comparer {

        public static int CompareDistance(
            MaskDataDistances x, MaskDataDistances y) {
            if (x == null) {
                if (y == null) {
                    return 0;
                } else {
                    return -1;
                }
            } else {
                if (y == null) {
                    return 1;
                } else {
                    int compare = x.distance.CompareTo(y.distance);

                    if (compare != 0) {
                        return compare;
                    } else {
                        return x.maskDataIndex.CompareTo(y.maskDataIndex);
                    }
                }
            }
        }
    }
}