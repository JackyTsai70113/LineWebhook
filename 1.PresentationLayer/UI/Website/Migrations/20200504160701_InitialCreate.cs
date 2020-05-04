using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyQuotes",
                columns: table => new
                {
                    Date = table.Column<DateTime>(type: "date", nullable: false),
                    CreateDateTime = table.Column<DateTime>(nullable: false),
                    StockCode = table.Column<string>(maxLength: 8, nullable: false),
                    TradeVolume = table.Column<int>(nullable: false),
                    Transaction = table.Column<int>(nullable: false),
                    TradeValue = table.Column<int>(nullable: false),
                    OpeningPrice = table.Column<float>(nullable: false),
                    HighestPrice = table.Column<float>(nullable: false),
                    LowestPrice = table.Column<float>(nullable: false),
                    ClosingPrice = table.Column<float>(nullable: false),
                    Direction = table.Column<int>(nullable: false),
                    Change = table.Column<float>(nullable: false),
                    LastBestBidPrice = table.Column<float>(nullable: false),
                    LastBestBidVolume = table.Column<int>(nullable: false),
                    LastBestAskPrice = table.Column<float>(nullable: false),
                    LastBestAskVolume = table.Column<int>(nullable: false),
                    PriceEarningRatio = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyQuotes", x => x.Date);
                    table.UniqueConstraint("AK_DailyQuotes_Date_StockCode", x => new { x.Date, x.StockCode });
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyQuotes");
        }
    }
}
