using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Privacy_for_Chats_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsOnlyJoin",
                table: "UserChats");

            migrationBuilder.AddColumn<bool>(
                name: "IsUserJoined",
                table: "UserChats",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlyJoin",
                table: "Chats",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserJoined",
                table: "UserChats");

            migrationBuilder.DropColumn(
                name: "IsOnlyJoin",
                table: "Chats");

            migrationBuilder.AddColumn<bool>(
                name: "IsOnlyJoin",
                table: "UserChats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
