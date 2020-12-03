using Microsoft.EntityFrameworkCore.Migrations;

namespace metrics.Data.Sql.Migrations
{
    public partial class vkuser : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "VkUserModel",
                columns: table => new
                {
                    Id = table.Column<int>(type: "integer", nullable: false),
                    Avatar = table.Column<string>(type: "text", nullable: false),
                    FullName = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VkUserModel", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VkUserModel");
        }
    }
}
