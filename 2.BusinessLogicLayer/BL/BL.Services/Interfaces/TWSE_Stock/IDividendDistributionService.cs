using Core.Domain.Entities.TWSE_Stock;
using System;
using System.Collections.Generic;

namespace BL.Services.Interfaces.TWSE_Stock {

    public interface IDividendDistributionService {

        List<DividendDistribution> CrawlDividendDistributionListByStockCodeEnumArray(Array stockCodeEnums = null);
    }
}