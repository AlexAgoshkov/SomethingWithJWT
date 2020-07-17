using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Chat_Updated_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Files");

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Users",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Messages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ImagePath",
                table: "Files",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "ImageId",
                table: "Chats",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$0qcC1DKsbZq9f47lvesltdSeE49X8utUlaxJjb4awzowsU/P");

            migrationBuilder.CreateIndex(
                name: "IX_Users_ImageId",
                table: "Users",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Messages_ImageId",
                table: "Messages",
                column: "ImageId");

            migrationBuilder.CreateIndex(
                name: "IX_Chats_ImageId",
                table: "Chats",
                column: "ImageId");

            migrationBuilder.AddForeignKey(
                name: "FK_Chats_Files_ImageId",
                table: "Chats",
                column: "ImageId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Messages_Files_ImageId",
                table: "Messages",
                column: "ImageId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Users_Files_ImageId",
                table: "Users",
                column: "ImageId",
                principalTable: "Files",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Chats_Files_ImageId",
                table: "Chats");

            migrationBuilder.DropForeignKey(
                name: "FK_Messages_Files_ImageId",
                table: "Messages");

            migrationBuilder.DropForeignKey(
                name: "FK_Users_Files_ImageId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Users_ImageId",
                table: "Users");

            migrationBuilder.DropIndex(
                name: "IX_Messages_ImageId",
                table: "Messages");

            migrationBuilder.DropIndex(
                name: "IX_Chats_ImageId",
                table: "Chats");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Messages");

            migrationBuilder.DropColumn(
                name: "ImagePath",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ImageId",
                table: "Chats");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Files",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$xmQIqr5Q3kWvFEYOdtWwCJi+kZd3+4YlZ8SYt/5MjdIYlCcl");
        }
    }
}
