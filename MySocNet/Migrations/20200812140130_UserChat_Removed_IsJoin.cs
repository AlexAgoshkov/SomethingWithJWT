using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class UserChat_Removed_IsJoin : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserJoined",
                table: "UserChats");

            migrationBuilder.AddColumn<bool>(
                name: "IsUserJoined",
                table: "ChatMembers",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsUserJoined",
                table: "ChatMembers");

            migrationBuilder.AddColumn<bool>(
                name: "IsUserJoined",
                table: "UserChats",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }
    }
}
