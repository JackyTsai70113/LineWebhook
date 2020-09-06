using Core.Domain.DTO.MaskInstitution;
using DA.Managers.MaskInstitution;
using System;
using System.Collections.Generic;

namespace BL.Services.MaskInstitution {

    public class MaskInstitutionService {

        public int GetCount() {
            var maskInstitutionList = MaskInstitutionSourceManager.GetList();
            return maskInstitutionList.Count;
        }

        public List<MaskData> GetTopMaskDatasByComputingDistance(string location, int count = int.MaxValue) {
            var maskDatas = MaskInstitutionManager.GetTopMaskDatasBySecondDivision(location, count);
            return maskDatas;
        }
    }
}