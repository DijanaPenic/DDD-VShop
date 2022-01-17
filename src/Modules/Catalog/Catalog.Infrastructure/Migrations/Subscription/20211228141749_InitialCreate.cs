using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.Migrations.Subscription
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
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_checkpoint", x => x.subscription_id);
                });

            migrationBuilder.CreateTable(
                name: "message_dead_letter_log",
                schema: "subscription",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    stream_id = table.Column<string>(type: "text", nullable: false),
                    message_type = table.Column<string>(type: "text", nullable: false),
                    message_id = table.Column<Guid>(type: "uuid", nullable: false),
                    message_data = table.Column<string>(type: "text", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    error = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_dead_letter_log", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "checkpoint",
                schema: "subscription");

            migrationBuilder.DropTable(
                name: "message_dead_letter_log",
                schema: "subscription");
        }
    }
}
