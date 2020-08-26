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

        public List<MaskData> GetTopMaskDatasByComputingDistance(string location, int count = Int32.MaxValue) {
            var maskDatas = MaskInstitutionManager.GetTopMaskDatasByComputingDistance(location, count);
            return maskDatas;
        }
    }
}