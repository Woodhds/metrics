using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace metrics.Data.Sql.Migrations
{
    public partial class start : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageCategory",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    Title = table.Column<string>(maxLength: 255, nullable: true),
                    Color = table.Column<string>(maxLength: 100, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageCategory", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "VkRepost",
                columns: table => new
                {
                    Id = table.Column<int>(nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    UserId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    MessageId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    DateStatus = table.Column<DateTime>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkRepost", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "MessageVk",
                columns: table => new
                {
                    MessageId = table.Column<int>(nullable: false),
                    OwnerId = table.Column<int>(nullable: false),
                    MessageCategoryId = table.Column<int>(nullable: false),
                    Status = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageVk", x => new { x.MessageId, x.OwnerId });
                    table.ForeignKey(
                        name: "FK_MessageVk_MessageCategory_MessageCategoryId",
                        column: x => x.MessageCategoryId,
                        principalTable: "MessageCategory",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_MessageVk_MessageCategoryId",
                table: "MessageVk",
                column: "MessageCategoryId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageVk");

            migrationBuilder.DropTable(
                name: "VkRepost");

            migrationBuilder.DropTable(
                name: "MessageCategory");
        }
    }
}
