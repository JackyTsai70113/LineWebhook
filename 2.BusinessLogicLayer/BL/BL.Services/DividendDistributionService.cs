using BL.Services.Base;
using BL.Services.Interfaces.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using DA.Managers.Interfaces.TWSE_Stock;
using DA.Managers.TWSE_Stock;
using System;
using System.Collections.Generic;

namespace BL.Services {

    public class DividendDistributionService : BaseService, IDividendDistributionService {

        public DividendDistributionService() {
            DividendDistributionManager = new DividendDistributionManager();
        }

        /// <summary>
        /// IDividendDistributionManager介面
        /// </summary>
        public IDividendDistributionManager DividendDistributionManager { get; set; }

        public List<DividendDistribution> CrawlDividendDistributionListByStockCodeEnumArray(Array stockCodeEnums = null) {
            List<DividendDistribution> dividendDistributionList = DividendDistributionManager.CrawlDividendDistribution(
                new StockCodeEnum[] {
                    StockCodeEnum._2884
                    , StockCodeEnum._2885
                });
            return dividendDistributionList;
        }
    }
}