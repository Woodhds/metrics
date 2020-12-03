using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

namespace metrics.Data.Sql.Migrations
{
    public partial class concurrency : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkRepostUserOffset");

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "VkRepost",
                type: "xid",
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                table: "MessageCategory",
                type: "xid",
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                table: "VkRepost");

            migrationBuilder.DropColumn(
                name: "xmin",
                table: "MessageCategory");

            migrationBuilder.CreateTable(
                name: "VkRepostUserOffset",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    LastPost = table.Column<DateTimeOffset>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkRepostUserOffset", x => x.UserId);
                });
        }
    }
}
