using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.Migrations.Sales
{
    public partial class EnabledXminConcurrenyToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);

            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "shopping_cart",
                table: "shopping_cart_info");
        }
    }
}
