using Microsoft.EntityFrameworkCore.Migrations;

namespace Website.Migrations {
    public partial class Update_DailyQuotes_LastBestAskPrice_To_bigint : Migration {
        protected override void Up(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<long>(
                name: "TradeValue",
                table: "DailyQuotes",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int");
        }

        protected override void Down(MigrationBuilder migrationBuilder) {
            migrationBuilder.AlterColumn<int>(
                name: "TradeValue",
                table: "DailyQuotes",
                type: "int",
                nullable: false,
                oldClrType: typeof(long));
        }
    }
}
