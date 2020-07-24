using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Added_Msg_3 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserMessageId",
                table: "Messages",
                nullable: false,
                defaultValue: 0);

        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_UserMessages_MessageId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "UserMessageId",
                table: "Messages");
        }
    }
}