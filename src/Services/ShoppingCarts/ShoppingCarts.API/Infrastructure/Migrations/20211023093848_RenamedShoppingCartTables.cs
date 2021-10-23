using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.Migrations
{
    public partial class RenamedShoppingCartTables : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shopping_cart_details_product_item_shopping_cart_details_sh",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_details_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_details",
                schema: "shopping_cart",
                table: "shopping_cart_details");

            migrationBuilder.RenameTable(
                name: "shopping_cart_details_product_item",
                schema: "shopping_cart",
                newName: "shopping_cart_info_product_item",
                newSchema: "shopping_cart");

            migrationBuilder.RenameTable(
                name: "shopping_cart_details",
                schema: "shopping_cart",
                newName: "shopping_cart_info",
                newSchema: "shopping_cart");

            migrationBuilder.RenameIndex(
                name: "ix_shopping_cart_details_product_item_shopping_cart_info_id",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                newName: "ix_shopping_cart_info_product_item_shopping_cart_info_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_info",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shopping_cart_info_product_item_shopping_cart_info_shopping",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                column: "shopping_cart_info_id",
                principalSchema: "shopping_cart",
                principalTable: "shopping_cart_info",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "fk_shopping_cart_info_product_item_shopping_cart_info_shopping",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropPrimaryKey(
                name: "pk_shopping_cart_info",
                schema: "shopping_cart",
                table: "shopping_cart_info");

            migrationBuilder.RenameTable(
                name: "shopping_cart_info_product_item",
                schema: "shopping_cart",
                newName: "shopping_cart_details_product_item",
                newSchema: "shopping_cart");

            migrationBuilder.RenameTable(
                name: "shopping_cart_info",
                schema: "shopping_cart",
                newName: "shopping_cart_details",
                newSchema: "shopping_cart");

            migrationBuilder.RenameIndex(
                name: "ix_shopping_cart_info_product_item_shopping_cart_info_id",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item",
                newName: "ix_shopping_cart_details_product_item_shopping_cart_info_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_details_product_item",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item",
                column: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_shopping_cart_details",
                schema: "shopping_cart",
                table: "shopping_cart_details",
                column: "id");

            migrationBuilder.AddForeignKey(
                name: "fk_shopping_cart_details_product_item_shopping_cart_details_sh",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item",
                column: "shopping_cart_info_id",
                principalSchema: "shopping_cart",
                principalTable: "shopping_cart_details",
                principalColumn: "id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
