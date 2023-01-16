using Core.Domain.Entities.TestDB;
using Microsoft.EntityFrameworkCore;

namespace Website.Data {

    public class LineWebhookContext : DbContext {

        public LineWebhookContext(DbContextOptions<LineWebhookContext> options)
            : base(options) {
        }

        public DbSet<Inventory> Inventories { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder) {
        }
    }
}