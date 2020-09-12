using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations {
    public partial class Add_YearlyTradingInformation : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.CreateTable(
                name: "StockValueEstimations",
                columns: table => new {
                    StockCode = table.Column<string>(maxLength: 8, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: false),
                    RecentDividends = table.Column<float>(nullable: false),
                    DividendsIn5Years = table.Column<float>(nullable: false),
                    DividendsIn10Years = table.Column<float>(nullable: false),
                    CheapPriceByRecentDividends = table.Column<float>(nullable: false),
                    ReasonablePriceByRecentDividends = table.Column<float>(nullable: false),
                    ExpensivePriceByRecentDividends = table.Column<float>(nullable: false),
                    CheapPriceByDividendsIn5Years = table.Column<float>(nullable: false),
                    ReasonablePriceByDividendsIn5Years = table.Column<float>(nullable: false),
                    ExpensivePriceByDividendsIn5Years = table.Column<float>(nullable: false),
                    CheapPriceByDividendsIn10Years = table.Column<float>(nullable: false),
                    ReasonablePriceByDividendsIn10Years = table.Column<float>(nullable: false),
                    ExpensivePriceByDividendsIn10Years = table.Column<float>(nullable: false),
                    CheapPriceByStockPriceOver10Years = table.Column<float>(nullable: false),
                    ReasonablePriceByStockPriceOver10Years = table.Column<float>(nullable: false),
                    ExpensivePriceByStockPriceOver10Years = table.Column<float>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_StockValueEstimations", x => x.StockCode);
                });

            migrationBuilder.CreateTable(
                name: "YearlyTradingInformations",
                columns: table => new {
                    Year = table.Column<short>(type: "smallint", nullable: false),
                    StockCode = table.Column<string>(maxLength: 8, nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    UpdateDateTime = table.Column<DateTime>(nullable: false),
                    TradeVolume = table.Column<long>(nullable: false),
                    TradeValue = table.Column<long>(nullable: false),
                    Transaction = table.Column<int>(nullable: false),
                    HighestPrice = table.Column<float>(nullable: false),
                    HighestPriceDate = table.Column<DateTime>(type: "date", nullable: false),
                    LowestPrice = table.Column<float>(nullable: false),
                    LowestPriceDate = table.Column<DateTime>(type: "date", nullable: false),
                    AverageClosingPrice = table.Column<float>(nullable: false)
                },
                constraints: table => {
                    table.PrimaryKey("PK_YearlyTradingInformations", x => new { x.Year, x.StockCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropTable(
                name: "StockValueEstimations");

            migrationBuilder.DropTable(
                name: "YearlyTradingInformations");
        }
    }
}
