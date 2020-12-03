using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace metrics.Data.Sql.Migrations
{
    public partial class datetimeoffset : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "LastPost",
                table: "VkRepostUserOffset",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AlterColumn<DateTimeOffset>(
                name: "DateStatus",
                table: "VkRepost",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<DateTime>(
                name: "LastPost",
                table: "VkRepostUserOffset",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));

            migrationBuilder.AlterColumn<DateTime>(
                name: "DateStatus",
                table: "VkRepost",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(DateTimeOffset));
        }
    }
}
