using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Added_Detects_List : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Detects_DetectId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_DetectId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DetectId",
                table: "Users");

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Detects",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Detects_UserId",
                table: "Detects",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Detects_Users_UserId",
                table: "Detects",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Detects_Users_UserId",
                table: "Detects");

            migrationBuilder.DropIndex(
                name: "IX_Detects_UserId",
                table: "Detects");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Detects");

            migrationBuilder.AddColumn<int>(
                name: "DetectId",
                table: "Users",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Users_DetectId",
                table: "Users",
                column: "DetectId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Detects_DetectId",
                table: "Users",
                column: "DetectId",
                principalTable: "Detects",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
