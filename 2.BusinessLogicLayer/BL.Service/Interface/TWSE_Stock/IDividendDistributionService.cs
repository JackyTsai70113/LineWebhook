using Core.Domain.Entities.TWSE_Stock;
using System;
using System.Collections.Generic;

namespace BL.Service.Interface.TWSE_Stock {

    public interface IDividendDistributionService {

        List<DividendDistribution> CrawlDividendDistributionListByStockCodeEnumArray(Array stockCodeEnums = null);
    }
}