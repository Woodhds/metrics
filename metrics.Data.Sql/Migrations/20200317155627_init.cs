using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace metrics.Data.Sql.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserToken",
                columns: table => new
                {
                    UserId = table.Column<int>(nullable: false),
                    Token = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserToken", x => x.UserId);
                });

            migrationBuilder.CreateTable(
                name: "JoinGroup",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RowVersion = table.Column<long>(nullable: false),
                    GroupId = table.Column<int>(nullable: false),
                    UserTokenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JoinGroup", x => x.Id);
                    table.ForeignKey(
                        name: "FK_JoinGroup_UserToken_UserTokenId",
                        column: x => x.UserTokenId,
                        principalTable: "UserToken",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Repost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    RowVersion = table.Column<long>(nullable: false),
                    PostId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    UserTokenId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Repost", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Repost_UserToken_UserTokenId",
                        column: x => x.UserTokenId,
                        principalTable: "UserToken",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_JoinGroup_UserTokenId",
                table: "JoinGroup",
                column: "UserTokenId");

            migrationBuilder.CreateIndex(
                name: "IX_Repost_UserTokenId",
                table: "Repost",
                column: "UserTokenId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JoinGroup");

            migrationBuilder.DropTable(
                name: "Repost");

            migrationBuilder.DropTable(
                name: "UserToken");
        }
    }
}
