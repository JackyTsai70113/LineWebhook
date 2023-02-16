using Core.Domain.Entities.TWSE_Stock;

namespace BL.Service.Interface.TWSE_Stock
{
    public interface IDividendDistributionService
    {
        List<DividendDistribution> CrawlDividendDistributionListByStockCodeEnumArray(Array stockCodeEnums = null);
    }
}