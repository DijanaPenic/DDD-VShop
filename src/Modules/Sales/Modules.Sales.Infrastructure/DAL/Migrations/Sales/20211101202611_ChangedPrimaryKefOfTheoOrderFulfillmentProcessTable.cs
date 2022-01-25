using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Sales
{
    public partial class ChangedPrimaryKefOfTheoOrderFulfillmentProcessTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_order_fulfillment_process",
                schema: "order",
                table: "order_fulfillment_process");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_fulfillment_process",
                schema: "order",
                table: "order_fulfillment_process",
                column: "shopping_cart_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_order_fulfillment_process",
                schema: "order",
                table: "order_fulfillment_process");

            migrationBuilder.AddPrimaryKey(
                name: "pk_order_fulfillment_process",
                schema: "order",
                table: "order_fulfillment_process",
                column: "order_id");
        }
    }
}
