using Core.Domain.Entities.TWSE_Stock;
using Core.Domain.Entities.TWSE_Stock.Exchange;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Website.Data {

    public class LineWebhookContext : DbContext {

        public LineWebhookContext(DbContextOptions<LineWebhookContext> options)
            : base(options) {
        }

        public DbSet<DailyQuote> DailyQuotes { get; set; }
        public DbSet<DividendDistribution> DividendDistributions { get; set; }
        public DbSet<StockValueEstimation> StockValueEstimations { get; set; }
        public DbSet<YearlyTradingInformation> YearlyTradingInformations { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DailyQuote>().HasKey(dq => new { dq.Date, dq.StockCode });
            modelBuilder.Entity<DividendDistribution>().HasKey(dd => new { dd.Year, dd.StockCode });
            modelBuilder.Entity<YearlyTradingInformation>().HasKey(dd => new { dd.Year, dd.StockCode });
        }
    }
}