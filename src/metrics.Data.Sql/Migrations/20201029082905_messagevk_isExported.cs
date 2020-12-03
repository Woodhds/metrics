using Microsoft.EntityFrameworkCore.Migrations;

namespace metrics.Data.Sql.Migrations
{
    public partial class messagevk_isExported : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsExported",
                table: "MessageVk",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsExported",
                table: "MessageVk");
        }
    }
}
