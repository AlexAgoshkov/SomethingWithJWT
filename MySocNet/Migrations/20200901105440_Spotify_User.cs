using Microsoft.EntityFrameworkCore.Migrations;

namespace MySocNet.Migrations
{
    public partial class Spotify_User : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SpotifyProfileId",
                table: "Users",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "ExternalUrls",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Spotify = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ExternalUrls", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Followers",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Href = table.Column<string>(nullable: true),
                    Total = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Followers", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "SpotifyUsers",
                columns: table => new
                {
                    UserSpotifyId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    DisplayName = table.Column<string>(nullable: true),
                    ExternalUrlsId = table.Column<int>(nullable: true),
                    FollowersId = table.Column<int>(nullable: true),
                    Href = table.Column<string>(nullable: true),
                    Id = table.Column<string>(nullable: true),
                    Type = table.Column<string>(nullable: true),
                    Uri = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SpotifyUsers", x => x.UserSpotifyId);
                    table.ForeignKey(
                        name: "FK_SpotifyUsers_ExternalUrls_ExternalUrlsId",
                        column: x => x.ExternalUrlsId,
                        principalTable: "ExternalUrls",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_SpotifyUsers_Followers_FollowersId",
                        column: x => x.FollowersId,
                        principalTable: "Followers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ProfileImage",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Height = table.Column<string>(nullable: true),
                    Url = table.Column<string>(nullable: true),
                    Width = table.Column<string>(nullable: true),
                    SpotifyUserUserSpotifyId = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProfileImage", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProfileImage_SpotifyUsers_SpotifyUserUserSpotifyId",
                        column: x => x.SpotifyUserUserSpotifyId,
                        principalTable: "SpotifyUsers",
                        principalColumn: "UserSpotifyId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Users_SpotifyProfileId",
                table: "Users",
                column: "SpotifyProfileId");

            migrationBuilder.CreateIndex(
                name: "IX_ProfileImage_SpotifyUserUserSpotifyId",
                table: "ProfileImage",
                column: "SpotifyUserUserSpotifyId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotifyUsers_ExternalUrlsId",
                table: "SpotifyUsers",
                column: "ExternalUrlsId");

            migrationBuilder.CreateIndex(
                name: "IX_SpotifyUsers_FollowersId",
                table: "SpotifyUsers",
                column: "FollowersId");

            migrationBuilder.AddForeignKey(
                name: "FK_Users_SpotifyUsers_SpotifyProfileId",
                table: "Users",
                column: "SpotifyProfileId",
                principalTable: "SpotifyUsers",
                principalColumn: "UserSpotifyId",
                onDelete: ReferentialAction.Restrict);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
          
            migrationBuilder.DropForeignKey(
                name: "FK_Users_SpotifyUsers_SpotifyProfileId",
                table: "Users");

            migrationBuilder.DropTable(
                name: "ProfileImage");

            migrationBuilder.DropTable(
                name: "SpotifyUsers");

            migrationBuilder.DropTable(
                name: "ExternalUrls");

            migrationBuilder.DropTable(
                name: "Followers");

            migrationBuilder.DropIndex(
                name: "IX_Users_SpotifyProfileId",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "SpotifyProfileId",
                table: "Users");
        }
    }
}
