using Core.Domain.DTO.MaskInstitution;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DA.Managers.MaskInstitution {

    public class MaskInstitutionManager {
        private static List<MaskData> maskDataList;

        public MaskInstitutionManager() {
            maskDataList = MaskInstitutionSourceManager.GetList();
        }

        public static List<MaskData> GetMaskDatasFromLocationSuffix(string locationSuffix) {
            if (maskDataList == null) {
                maskDataList = MaskInstitutionSourceManager.GetList();
            }

            List<MaskData> result = new List<MaskData>();

            int strLength = locationSuffix.Length;
            foreach (var maskData in maskDataList) {
                if (maskData.Address.Substring(0, strLength) == locationSuffix) {
                    result.Add(maskData);
                }
            }
            return result;
        }

        public static List<MaskData> GetTopMaskDatasFromLocationSuffix(string locationSuffix, int count = Int32.MaxValue) {
            if (maskDataList == null) {
                maskDataList = MaskInstitutionSourceManager.GetList();
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
            string SecondDivision = GetLocationSecondDivision(location);
            List<MaskData> maskDataList = GetTopMaskDatasFromLocationSuffix(SecondDivision, count);
            return maskDataList.ToList();
        }

        public static string GetLocationFirstDivision(string address) {
            // 去除郵遞區號及台灣兩字
            int indexOfTaiwan = address.IndexOf("台灣");
            if (indexOfTaiwan != -1) {
                address = address.Substring(indexOfTaiwan + 2);
            }

            int indexOfFirstDivision = -1;
            if (address.Contains("市")) {
                indexOfFirstDivision = address.IndexOf("市");
            } else if (address.Contains("縣")) {
                indexOfFirstDivision = address.IndexOf("縣");
            }

            return address.Substring(0, indexOfFirstDivision + 1);
        }

        private static string GetLocationSecondDivision(string address) {
            // 去除郵遞區號及台灣兩字
            int indexOfTaiwan = address.IndexOf("台灣");
            if (indexOfTaiwan != -1) {
                address = address.Substring(indexOfTaiwan + 2);
            }

            int indexOfSecondDivision = -1;
            if (address.Contains("區")) {
                indexOfSecondDivision = address.IndexOf("區");
            } else if (address.Contains("鄉")) {
                indexOfSecondDivision = address.IndexOf("鄉");
            } else if (address.Contains("鎮")) {
                indexOfSecondDivision = address.IndexOf("鎮");
            } else if (address.IndexOf("縣") != -1 && address.Contains("市")) {
                indexOfSecondDivision = address.IndexOf("市");
            }

            return address.Substring(0, indexOfSecondDivision + 1);
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