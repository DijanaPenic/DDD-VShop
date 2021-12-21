using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.Migrations
{
    public partial class FixedPrimaryKeyForTheShoppingCartInfoItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                columns: new[] { "product_id", "shopping_cart_info_id" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                column: "product_id");
        }
    }
}
