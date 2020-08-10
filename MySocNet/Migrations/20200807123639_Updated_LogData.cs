using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Updated_LogData : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Chat",
                table: "LogData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ChatId",
                table: "LogData",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "User",
                table: "LogData",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LogData",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Chat",
                table: "LogData");

            migrationBuilder.DropColumn(
                name: "ChatId",
                table: "LogData");

            migrationBuilder.DropColumn(
                name: "User",
                table: "LogData");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "LogData");
        }
    }
}
