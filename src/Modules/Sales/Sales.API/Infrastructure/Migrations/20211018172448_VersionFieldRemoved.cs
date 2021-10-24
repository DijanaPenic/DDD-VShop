using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.API.Infrastructure.Migrations
{
    public partial class VersionFieldRemoved : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "version",
                schema: "basket",
                table: "basket_details");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "version",
                schema: "basket",
                table: "basket_details",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
