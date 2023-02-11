using NodaTime;
using YahooQuotesApi;

namespace BL.Service.Stock
{
    public class StockService
    {
        public static PriceTick[] GetPriceTicks(string symbol = null)
        {
            YahooQuotes yahooQuotes = new YahooQuotesBuilder()
                .WithHistoryStartDate(Instant.FromUtc(2023, 1, 1, 0, 0))
                .Build();

            symbol ??= "MSFT";
            Security security = yahooQuotes.GetAsync(symbol, Histories.PriceHistory).Result ??
                throw new ArgumentException($"Unknown symbol: {symbol}.");

            PriceTick[] priceTicks = security.PriceHistory.Value;

            return priceTicks;
        }
    }
}