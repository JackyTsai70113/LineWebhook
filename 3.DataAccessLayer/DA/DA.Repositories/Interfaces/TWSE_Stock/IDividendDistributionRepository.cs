using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Interfaces.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Repositories.Interfaces.TWSE_Stock {

    public interface IDividendDistributionRepository : IBaseRepository {

        /// <summary>
        /// 若資料庫中無對應的股利分派，則 Insert，否則 Update
        /// </summary>
        /// <param name="dividendDistributionList">股利分派列表</param>
        /// <returns>成功數量</returns>
        int SaveDividendDistributionList(List<DividendDistribution> dividendDistributionList);
    }
}