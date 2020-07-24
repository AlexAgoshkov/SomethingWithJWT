using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Added_Msg_2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastChatDatas_Chats_ChatId",
                table: "LastChatDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_MessageId",
                table: "UserMessages");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "LastChatDatas",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages",
                columns: new[] { "MessageId", "UserId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_UserId",
                table: "UserMessages",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastChatDatas_Chats_ChatId",
                table: "LastChatDatas",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LastChatDatas_Chats_ChatId",
                table: "LastChatDatas");

            migrationBuilder.DropPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages");

            migrationBuilder.DropIndex(
                name: "IX_UserMessages_UserId",
                table: "UserMessages");

            migrationBuilder.DropColumn(
                name: "Text",
                table: "LastChatDatas");

            migrationBuilder.DropColumn(
                name: "UserName",
                table: "LastChatDatas");

            migrationBuilder.AlterColumn<int>(
                name: "ChatId",
                table: "LastChatDatas",
                type: "int",
                nullable: true,
                oldClrType: typeof(int));

            migrationBuilder.AddColumn<int>(
                name: "MessageId",
                table: "LastChatDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "UserId",
                table: "LastChatDatas",
                type: "int",
                nullable: true);

            migrationBuilder.AddPrimaryKey(
                name: "PK_UserMessages",
                table: "UserMessages",
                columns: new[] { "UserId", "MessageId" });

            migrationBuilder.CreateIndex(
                name: "IX_UserMessages_MessageId",
                table: "UserMessages",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LastChatDatas_MessageId",
                table: "LastChatDatas",
                column: "MessageId");

            migrationBuilder.CreateIndex(
                name: "IX_LastChatDatas_UserId",
                table: "LastChatDatas",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_LastChatDatas_Chats_ChatId",
                table: "LastChatDatas",
                column: "ChatId",
                principalTable: "Chats",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastChatDatas_Messages_MessageId",
                table: "LastChatDatas",
                column: "MessageId",
                principalTable: "Messages",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_LastChatDatas_Users_UserId",
                table: "LastChatDatas",
                column: "UserId",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
