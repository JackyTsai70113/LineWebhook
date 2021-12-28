using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations {
    public partial class Add_DividendDistribution : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "DividendDistributions",
                columns: table => new {
                    StockCode = table.Column<string>(maxLength: 8, nullable: false),
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: false),
                    CashDividendsToBeDistributedFromRetainedEarnings = table.Column<float>(nullable: false),
                    CashDividendsFromLegalReserveAndCapitalSurplus = table.Column<float>(nullable: false),
                    SharesDistributedFromEarnings = table.Column<float>(nullable: false),
                    SharesDistributedFromLegalReserveAndCapitalSurplus = table.Column<float>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_DividendDistributions", x => new { x.Year, x.StockCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "DividendDistributions");
        }
    }
}
