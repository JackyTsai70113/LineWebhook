using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations {
    public partial class UpdateStockCode : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyQuotes",
                table: "DailyQuotes");

            migrationBuilder.DropUniqueConstraint(
                name: "AK_DailyQuotes_Date_StockCode",
                table: "DailyQuotes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyQuotes",
                table: "DailyQuotes",
                columns: new[] { "Date", "StockCode" });
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.DropPrimaryKey(
                name: "PK_DailyQuotes",
                table: "DailyQuotes");

            migrationBuilder.AddPrimaryKey(
                name: "PK_DailyQuotes",
                table: "DailyQuotes",
                column: "Date");

            migrationBuilder.AddUniqueConstraint(
                name: "AK_DailyQuotes_Date_StockCode",
                table: "DailyQuotes",
                columns: new[] { "Date", "StockCode" });
        }
    }
}
