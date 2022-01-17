using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.Migrations.Subscription
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "subscription");

            migrationBuilder.CreateTable(
                name: "checkpoint",
                schema: "subscription",
                columns: table => new
                {
                    subscription_id = table.Column<string>(type: "text", nullable: false),
                    position = table.Column<decimal>(type: "numeric(20,0)", nullable: true),
                    date_created_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checkpoint", x => x.subscription_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkpoint",
                schema: "subscription");
        }
    }
}
