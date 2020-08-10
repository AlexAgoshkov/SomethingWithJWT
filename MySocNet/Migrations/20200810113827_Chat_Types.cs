using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Chat_Types : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "ChatType",
                table: "Chats",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlyJoin",
                table: "Chats",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivate",
                table: "Chats",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPrivateMask",
                table: "Chats",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ChatType",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsOnlyJoin",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsPrivate",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "IsPrivateMask",
                table: "Chats");
        }
    }
}
