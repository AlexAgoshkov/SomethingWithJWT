using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Chats : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "Chats",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$y9XfZMzx/PCx/bvctINy1Cix76lml2PwwDii3BGqFpjTx0K9");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_UserId",
                table: "Chats",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Users_UserId",
                table: "Chats");

            migrationBuilder.DropIndex(
                name: "IX_Chats_UserId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Chats");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$Mi0udo8tAaC0/LJjHV8JazZ4EJNlBqFKteUJBtgs6+/aVhkG");
        }
    }
}
