﻿using System.Collections.Generic;
using Core.Domain.DTO;

namespace BL.Services.Interfaces {

    public interface IMaskInstitutionService {
        /// <summary>
        /// 取得 口罩機構列表
        /// </summary>
        /// <param name="address">起始地址</param>
        /// <returns>口罩機構列表</returns>
        List<MaskInstitution> GetMaskInstitutions(string address);

        /// <summary>
        /// 以Api取得口罩機構數量
        /// </summary>
        /// <returns>口罩機構數量</returns>
        int GetMaskInstitutionCount();
    }
}