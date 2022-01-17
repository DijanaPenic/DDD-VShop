using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Infrastructure.Migrations.Sales
{
    public partial class UsedProductIdAsThePrimaryKeyInShoppingCartItemTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                column: "product_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                column: "id");
        }
    }
}
