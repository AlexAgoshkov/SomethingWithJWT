using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Added_LastData_M : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "LastChatDatas",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(nullable: true),
                    MessageId = table.Column<int>(nullable: true),
                    ChatId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_LastChatDatas", x => x.Id);
                    table.ForeignKey(
                        name: "FK_LastChatDatas_Chats_ChatId",
                        column: x => x.ChatId,
                        principalTable: "Chats",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LastChatDatas_Messages_MessageId",
                        column: x => x.MessageId,
                        principalTable: "Messages",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_LastChatDatas_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$rQTWW5QUrBo+cfXI1birqoETx5kXSvo+VbMj3HxfSMwn3oZ+");

            migrationBuilder.CreateIndex(
                name: "IX_LastChatDatas_ChatId",
                table: "LastChatDatas",
                column: "ChatId");

            migrationBuilder.CreateIndex(
                name: "IX_LastChatDatas_MessageId",
                table: "LastChatDatas",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LastChatDatas_UserId",
                table: "LastChatDatas",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "LastChatDatas");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "Password",
                value: "$MYHASH$V1$100$p2bYy56pCd8V5t98yY9eUlVh50R1XRf+duwBlLWuxryOACu+");
        }
    }
}
