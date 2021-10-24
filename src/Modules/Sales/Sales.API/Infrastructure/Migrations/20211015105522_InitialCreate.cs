using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Services.Sales.API.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "basket");

            migrationBuilder.CreateTable(
                name: "basket_details",
                schema: "basket",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    customer_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    version = table.Column<int>(type: "integer", nullable: false)
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "basket_details_product_item",
                schema: "basket");

            migrationBuilder.DropTable(
                name: "basket_details",
                schema: "basket");
        }
    }
}
