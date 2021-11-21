using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Billing.API.Infrastructure.Migrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "payment");

            migrationBuilder.CreateTable(
                name: "payment_transfer",
                schema: "payment",
                columns: table => new
                {
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_transfer", x => x.order_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_transfer",
                schema: "payment");
        }
    }
}
