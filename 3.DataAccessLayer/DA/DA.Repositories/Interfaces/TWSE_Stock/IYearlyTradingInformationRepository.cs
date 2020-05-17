using Core.Domain.Entities.TWSE_Stock.Exchange;
using Core.Domain.Interfaces.Repositories.Base;
using System;
using System.Collections.Generic;
using System.Text;

namespace DA.Repositories.Interfaces.TWSE_Stock {

    public interface IYearlyTradingInformationRepository : IBaseRepository {

        /// <summary>
        /// 若資料庫中無對應的年度交易資訊，則 Insert，否則 Update
        /// </summary>
        /// <param name="yearlyTradingInformationList">年度交易資訊列表</param>
        /// <returns>成功數量</returns>
        int SaveYearlyTradingInformationList(List<YearlyTradingInformation> yearlyTradingInformationList);
    }
}