﻿using BL.Service.Interface;
using Core.Domain.DTO;
using Core.Domain.Utilities;

namespace BL.Service
{
    public class MaskInstitutionService : IMaskInstitutionService
    {
        /// <summary>
        /// 取得 口罩機構列表
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>口罩機構列表</returns>
        public List<MaskInstitution> GetMaskInstitutions(string address)
        {
            string addressSecondDivision = AddressUtility.GetSecondDivision(address);
            return GetMaskInstitutionsFromAddressSuffix(addressSecondDivision);
        }

        /// <summary>
        /// 以Api取得口罩機構數量
        /// </summary>
        /// <returns>口罩機構數量</returns>
        public int GetMaskInstitutionCount()
        {
            var maskInstitutionStr = GetMaskInstitutionStr();
            return maskInstitutionStr.Count(f => f == '\n') - 1;
        }

        private static string GetMaskInstitutionStr()
        {
            string uri = "https://data.nhi.gov.tw/Datasets/Download.ashx?rid=A21030000I-D50001-001&l=https://data.nhi.gov.tw/resource/mask/maskdata.csv";
            return RequestUtility.GetStringFromGetRequest(uri);
        }

        /// <summary>
        /// 取得符合地址前綴的口罩機構列表
        /// </summary>
        /// <param name="addressSuffix">地址前綴</param>
        /// <param name="number">給定數量</param>
        /// <returns>口罩機構列表</returns>
        private static List<MaskInstitution> GetMaskInstitutionsFromAddressSuffix(string addressSuffix, int number = int.MaxValue)
        {
            IEnumerable<MaskInstitution> maskDatas = GetMaskInstitutions();
            List<MaskInstitution> result = new();
            int count = 0;
            int strLength = addressSuffix.Length;
            addressSuffix = addressSuffix.Replace("台", "臺");
            string maskDataAddressSuffix;
            foreach (var maskData in maskDatas)
            {
                if (strLength == 3)
                {
                    maskDataAddressSuffix = maskData.Address.Substring(3, strLength).Replace("台", "臺");
                }
                else
                {
                    maskDataAddressSuffix = maskData.Address[..strLength].Replace("台", "臺");
                }
                if (addressSuffix == maskDataAddressSuffix)
                {
                    result.Add(maskData);
                    count++;
                    if (count == number)
                    {
                        break;
                    }
                }
            }
            return result;
        }

        /// <summary>
        /// 以Api取得口罩機構迭代器
        /// </summary>
        /// <returns>口罩機構迭代器</returns>
        private static IEnumerable<MaskInstitution> GetMaskInstitutions()
        {
            string maskInstitutionStr = GetMaskInstitutionStr();
            string[] maskDataStrArr = maskInstitutionStr.Split('\n');
            for (int i = 1; i < maskDataStrArr.Length - 1; i++)
            {
                yield return ConvertToMaskInstitution(maskDataStrArr[i]);
            }
        }

        /// <summary>
        /// 轉換成口罩機構
        /// </summary>
        /// <param name="maskInstitutionStr">字串</param>
        /// <returns>口罩機構</returns>
        private static MaskInstitution ConvertToMaskInstitution(string maskInstitutionStr)
        {
            var maskInstitutionArr = maskInstitutionStr.Split(',');
            if (!int.TryParse(maskInstitutionArr[4], out int adultMasks))
            {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[4]} to Int.");
                adultMasks = int.MaxValue;
            }
            if (!int.TryParse(maskInstitutionArr[5], out int childMasks))
            {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[5]} to Int.");
                childMasks = int.MaxValue;
            }
            if (!DateTime.TryParse(maskInstitutionArr[6], out DateTime updateTime))
            {
                Console.WriteLine($"Ex: Cannot parse {maskInstitutionArr[6]} to Int.");
                updateTime = DateTime.MinValue;
            }
            return new MaskInstitution
            {
                Id = maskInstitutionArr[0],
                Name = maskInstitutionArr[1],
                Address = maskInstitutionArr[2],
                PhoneNumber = maskInstitutionArr[3],
                NumberOfAdultMasks = adultMasks,
                NumberOfChildMasks = childMasks,
                UpdateTime = updateTime
            };
        }
    }
}