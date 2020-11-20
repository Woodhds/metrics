using Microsoft.EntityFrameworkCore.Migrations;

namespace metrics.Data.Sql.Migrations
{
    public partial class messagecategory_add : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "SortOrder",
                table: "MessageCategory",
                type: "integer",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SortOrder",
                table: "MessageCategory");
        }
    }
}
