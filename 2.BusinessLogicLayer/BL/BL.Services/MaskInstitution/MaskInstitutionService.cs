using System;
using System.Collections.Generic;
using System.Linq;
using BL.Services.Interfaces;
using BL.Services.Map;
using Core.Domain.DTO.Map;
using Core.Domain.DTO.ResponseDTO;
using Core.Domain.Utilities;

namespace BL.Services {

    public class MaskInstitutionService : IMaskInstitutionService {
        private readonly IMapHereService _mapHereService;

        public MaskInstitutionService(IMapHereService mapHereService) {
            _mapHereService = mapHereService;
        }

        /// <summary>
        /// 取得 距離最短的口罩機構列表
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <param name="number">給定數量</param>
        /// <returns>口罩機構列表</returns>
        public List<MaskInstitution> GetMaskInstitutionsByComputingDistance(string address, int number = int.MaxValue) {
            string addressSecondDivision = AddressUtility.GetSecondDivision(address);
            var allMaskInstitutions = GetMaskInstitutionsFromAddressSuffix(addressSecondDivision);
            return GetMaskInstitutionsByComputingRelaticeDistance(address, allMaskInstitutions, number);
        }

        /// <summary>
        /// 以Api取得口罩機構數量
        /// </summary>
        /// <returns>口罩機構數量</returns>
        public int GetMaskInstitutionCount() {
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var maskDataStr = RequestUtility.GetStringFromGetRequest(uri);
            return maskDataStr.Count(f => f == '\n') - 1;
        }

        /// <summary>
        /// 取得符合地址前綴的口罩機構列表
        /// </summary>
        /// <param name="addressSuffix">地址前綴</param>
        /// <param name="number">給定數量</param>
        /// <returns>口罩機構列表</returns>
        private List<MaskInstitution> GetMaskInstitutionsFromAddressSuffix(string addressSuffix, int number = int.MaxValue) {
            IEnumerable<MaskInstitution> maskDataList = GetMaskInstitutions();

            List<MaskInstitution> result = new List<MaskInstitution>();
            int count = 0;

            int strLength = addressSuffix.Length;
            addressSuffix = addressSuffix.Replace("台", "臺");
            foreach (var maskData in maskDataList) {
                string maskDataAddressSuffix = maskData.Address.Substring(0, strLength).Replace("台", "臺");
                if (addressSuffix == maskDataAddressSuffix) {
                    result.Add(maskData);
                    count++;
                    if (count == number) {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 根據距離，取得給定數量的口罩資訊
        /// </summary>
        /// <param name="sourceAddress">起始地址</param>
        /// <param name="maskInstitutions">口罩機構列表</param>
        /// <param name="count">給定數量</param>
        /// <returns>口罩機構列表</returns>
        private List<MaskInstitution> GetMaskInstitutionsByComputingRelaticeDistance(
            string sourceAddress, List<MaskInstitution> maskInstitutions, int count = int.MaxValue) {
            LatLng sourceLatLng = _mapHereService.GetLatLngFromAddress(sourceAddress);
            List<MaskInstitution> result = new List<MaskInstitution>();
            result = maskInstitutions.OrderBy(mask => {
                LatLng targetLatLng = _mapHereService.GetLatLngFromAddress(mask.Address);
                return _mapHereService.GetTravelTimeFromTwoLatLngs(sourceLatLng, targetLatLng);
            }).Take(count).ToList();
            return result;
        }

        /// <summary>
        /// 以Api取得口罩機構迭代器
        /// </summary>
        /// <returns>口罩機構迭代器</returns>
        private IEnumerable<MaskInstitution> GetMaskInstitutions() {
            string uri = "http://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            var maskDataStr = RequestUtility.GetStringFromGetRequest(uri);
            var maskDataStrArr = maskDataStr.Split('\n');
            for (int i = 1; i < maskDataStrArr.Length - 1; i++) {
                yield return ConvertToMaskInstitution(maskDataStrArr[i]);
            }
        }

        /// <summary>
        /// 轉換成口罩機構
        /// </summary>
        /// <param name="maskInstitutionStr">字串</param>
        /// <returns>口罩機構</returns>
        private MaskInstitution ConvertToMaskInstitution(string maskInstitutionStr) {
            var maskInstitutionArr = maskInstitutionStr.Split(',');
            if (!int.TryParse(maskInstitutionArr[4], out int adultMasks)) {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[4]} to Int.");
                adultMasks = int.MaxValue;
            }
            if (!int.TryParse(maskInstitutionArr[5], out int childMasks)) {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[5]} to Int.");
                childMasks = int.MaxValue;
            }
            if (!DateTime.TryParse(maskInstitutionArr[6], out DateTime updateTime)) {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[6]} to Int.");
                updateTime = DateTime.MinValue;
            }
            return new MaskInstitution {
                Id = maskInstitutionArr[0],
                Name = maskInstitutionArr[1],
                Address = maskInstitutionArr[2],
                PhoneNumber = maskInstitutionArr[3],
                numberOfAdultMasks = adultMasks,
                numberOfChildMasks = childMasks,
                UpdateTime = updateTime
            };
        }
    }
}