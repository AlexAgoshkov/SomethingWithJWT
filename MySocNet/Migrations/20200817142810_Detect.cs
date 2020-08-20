using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Detect : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "DetectId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Detects",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DeviceType = table.Column<string>(nullable: true),
                    Os = table.Column<string>(nullable: true),
                    Browser = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Detects", x => x.Id);
                });

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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Users_Detects_DetectId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "Detects");

            migrationBuilder.DropIndex(
                name: "IX_Users_DetectId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "DetectId",
                table: "Users");
        }
    }
}
