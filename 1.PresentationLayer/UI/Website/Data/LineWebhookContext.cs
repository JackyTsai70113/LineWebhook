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

        public DbSet<Note> Note { get; set; }
    }
}