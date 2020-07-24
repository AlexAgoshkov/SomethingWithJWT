using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Added_Read_Msg : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Friends",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.CreateTable(
                name: "UserMessages",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    MessageId = table.Column<int>(nullable: false),
                    IsRead = table.Column<bool>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserMessages", x => new { x.UserId, x.MessageId });
                    table.ForeignKey(
                        name: "FK_UserMessages_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserMessages_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_MessageId",
                table: "UserMessages",
                column: "MessageId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserMessages");

            migrationBuilder.AddColumn<bool>(
                name: "IsRead",
                table: "Messages",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "Id", "ActiveKeyId", "AuthenticationId", "Email", "FirstName", "ImageId", "Password", "SurName", "UserName", "UserRole" },
                values: new object[] { 1, null, null, "example@mail.hock", "Larry", null, "$MYHASH$V1$100$ErNqvAaMQ4D8boJ8FZ60aTcU+cC61ff/yvnda+X93M5ucygT", "Richi", "ggg", "Admin" });

            migrationBuilder.InsertData(
                table: "Friends",
                columns: new[] { "Id", "UserAddedId", "UserId" },
                values: new object[] { 1, 1, 1 });
        }
    }
}
