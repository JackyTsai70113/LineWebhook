using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Enums;
using System;
using System.Collections.Generic;

namespace DA.Managers.Interfaces.TWSE_Stock {

    public interface IDividendDistributionManager {

        List<DividendDistribution> CrawlDividendDistribution(StockCodeEnum stockCodeEnum);

        List<DividendDistribution> CrawlDividendDistribution(StockCodeEnum[] stockCodeEnums);
    }
}