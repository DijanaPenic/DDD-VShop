using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Core.DAL.Migrations.Sales
{
    public partial class RemovedOrderFulfillmentProcessManagerTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_fulfillment_process",
                schema: "order");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "order");

            migrationBuilder.CreateTable(
                name: "order_fulfillment_process",
                schema: "order",
                columns: table => new
                {
                    shopping_cart_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    description = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_fulfillment_process", x => x.shopping_cart_id);
                });
        }
    }
}
