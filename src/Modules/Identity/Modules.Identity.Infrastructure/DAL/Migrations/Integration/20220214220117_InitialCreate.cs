using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Identity.Infrastructure.DAL.Migrations.Integration
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "integration_event_queue",
                schema: "integration",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    type_name = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    times_sent = table.Column<int>(type: "integer", nullable: false),
                    body = table.Column<byte[]>(type: "bytea", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: true),
                    causation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    correlation_id = table.Column<Guid>(type: "uuid", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_integration_event_queue", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integration_event_queue",
                schema: "integration");
        }
    }
}
