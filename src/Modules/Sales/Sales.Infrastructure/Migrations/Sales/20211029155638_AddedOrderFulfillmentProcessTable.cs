using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Infrastructure.Migrations.Sales
{
    public partial class AddedOrderFulfillmentProcessTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "order");

            migrationBuilder.CreateTable(
                name: "order_fulfillment_process",
                schema: "order",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_order_fulfillment_process", x => x.order_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "order_fulfillment_process",
                schema: "order");
        }
    }
}
