using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.Migrations.Billing
{
    public partial class AddedTypeColumnToPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment_transfer",
                schema: "payment");

            migrationBuilder.CreateTable(
                name: "payment",
                schema: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    type = table.Column<int>(type: "integer", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    error = table.Column<string>(type: "text", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "payment",
                schema: "payment");

            migrationBuilder.CreateTable(
                name: "payment_transfer",
                schema: "payment",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    error = table.Column<string>(type: "text", nullable: true),
                    order_id = table.Column<Guid>(type: "uuid", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_payment_transfer", x => x.id);
                });
        }
    }
}
