using BL.Services.Map;
using Core.Domain.DTO.MaskInstitution;
using DA.Managers.MaskInstitution;
using System.Collections.Generic;
using System.Linq;

namespace BL.Services.MaskInstitution {

    public class MaskInstitutionService {

        public int GetCount() {
            var maskInstitutionList = MaskInstitutionSourceManager.GetList();
            return maskInstitutionList.Count;
        }

        public List<MaskData> GetTopMaskDatasByComputingDistance(string location, int count = int.MaxValue) {
            var maskDatas = MaskInstitutionManager.GetMaskDatasBySecondDivision(location, count);
            maskDatas = new MaskInstitutionService().GetTopByComputingDistance(location, maskDatas, 5);
            return maskDatas;
        }

        public List<MaskData> GetTopByComputingDistance(string sourceAddress, List<MaskData> maskDatas, int count = int.MaxValue) {
            LatLng sourceLatLng = MapHereHelper.GetLatLngFromAddress(sourceAddress);
            List<MaskData> result = new List<MaskData>();
            result = maskDatas.OrderBy(mask => {
                LatLng targetLatLng = MapHereHelper.GetLatLngFromAddress(mask.Address);
                return MapHereHelper.GetTravelTimeFromTwoLatLngs(sourceLatLng, targetLatLng);
            }).Take(count).ToList();
            //List<MaskData> baa = maskDatas.OrderBy(maskDatas.Select(m => m.));
            //var maskDatas = MaskInstitutionManager.GetMaskDatasBySecondDivision(location, count);
            return result;
        }
    }
}