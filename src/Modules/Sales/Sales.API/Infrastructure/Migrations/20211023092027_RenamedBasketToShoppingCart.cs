using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.API.Infrastructure.Migrations
{
    public partial class RenamedBasketToShoppingCart : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basket_details_product_item",
                schema: "basket");

            migrationBuilder.DropTable(
                name: "basket_details",
                schema: "basket");

            migrationBuilder.EnsureSchema(
                name: "shopping_cart");

            migrationBuilder.CreateTable(
                name: "shopping_cart_details",
                schema: "shopping_cart",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_cart_details", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "shopping_cart_details_product_item",
                schema: "shopping_cart",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    shopping_cart_info_id = table.Column<Guid>(type: "uuid", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_shopping_cart_details_product_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_shopping_cart_details_product_item_shopping_cart_details_sh",
                        column: x => x.shopping_cart_info_id,
                        principalSchema: "shopping_cart",
                        principalTable: "shopping_cart_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_shopping_cart_details_product_item_shopping_cart_info_id",
                schema: "shopping_cart",
                table: "shopping_cart_details_product_item",
                column: "shopping_cart_info_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "shopping_cart_details_product_item",
                schema: "shopping_cart");

            migrationBuilder.DropTable(
                name: "shopping_cart_details",
                schema: "shopping_cart");

            migrationBuilder.EnsureSchema(
                name: "basket");

            migrationBuilder.CreateTable(
                name: "basket_details",
                schema: "basket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_basket_details", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "basket_details_product_item",
                schema: "basket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    basket_details_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    product_id = table.Column<Guid>(type: "uuid", nullable: false),
                    quantity = table.Column<int>(type: "integer", nullable: false),
                    unit_price = table.Column<decimal>(type: "numeric", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_basket_details_product_item", x => x.id);
                    table.ForeignKey(
                        name: "fk_basket_details_product_item_basket_details_basket_details_id",
                        column: x => x.basket_details_id,
                        principalSchema: "basket",
                        principalTable: "basket_details",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_basket_details_product_item_basket_details_id",
                schema: "basket",
                table: "basket_details_product_item",
                column: "basket_details_id");
        }
    }
}
