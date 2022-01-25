using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Sales
{
    public partial class AddedAdditionalColumnsToOrderFulfillmentProcessTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "description",
                schema: "order",
                table: "order_fulfillment_process",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "shopping_cart_id",
                schema: "order",
                table: "order_fulfillment_process",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "description",
                schema: "order",
                table: "order_fulfillment_process");

            migrationBuilder.DropColumn(
                name: "shopping_cart_id",
                schema: "order",
                table: "order_fulfillment_process");
        }
    }
}
