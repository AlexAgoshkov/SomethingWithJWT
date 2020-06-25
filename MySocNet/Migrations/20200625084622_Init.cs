using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Authentications",
                columns: table => new
                {
                    AuthenticationId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AccessToken = table.Column<string>(nullable: true),
                    RefreshToken = table.Column<string>(nullable: true),
                    Created = table.Column<DateTime>(nullable: false),
                    Expires = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Authentications", x => x.AuthenticationId);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserName = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: false),
                    FirstName = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    SurName = table.Column<string>(type: "varchar(25)", maxLength: 25, nullable: true),
                    Email = table.Column<string>(type: "varchar(50)", maxLength: 50, nullable: true),
                    Password = table.Column<string>(type: "nvarchar(max)", maxLength: 50, nullable: false),
                    UserRole = table.Column<string>(nullable: true),
                    AuthenticationId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Users", x => x.UserId);
                    table.ForeignKey(
                        name: "FK_Users_Authentications_AuthenticationId",
                        column: x => x.AuthenticationId,
                        principalTable: "Authentications",
                        principalColumn: "AuthenticationId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "Friends",
                columns: table => new
                {
                    FriendID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserAddedId = table.Column<int>(nullable: false),
                    UserID = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Friends", x => x.FriendID);
                    table.ForeignKey(
                        name: "FK_Friends_Users_UserID",
                        column: x => x.UserID,
                        principalTable: "Users",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "AuthenticationId", "Email", "FirstName", "Password", "SurName", "UserName", "UserRole" },
                values: new object[] { 1, null, "example@mail.hock", "Larry", "$MYHASH$V1$100$88rhIb3mtZN/YqguXRmq+UGDqEdZod08fsAusiJmBBZYqtp1", "Richi", "ggg", "Admin" });

            migrationBuilder.InsertData(
                table: "Friends",
                columns: new[] { "FriendID", "UserAddedId", "UserID" },
                values: new object[] { 1, 1, 1 });

            migrationBuilder.CreateIndex(
                name: "IX_Friends_UserID",
                table: "Friends",
                column: "UserID");

            migrationBuilder.CreateIndex(
                name: "IX_Users_AuthenticationId",
                table: "Users",
                column: "AuthenticationId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Friends");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Authentications");
        }
    }
}
