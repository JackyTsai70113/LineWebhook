using Core.Domain.Entities.TWSE_Stock.Exchange;
using Microsoft.EntityFrameworkCore;
using Models.DB;
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

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
            modelBuilder.Entity<DailyQuote>()
                .HasKey(c => new { c.Date, c.StockCode });
        }
    }
}