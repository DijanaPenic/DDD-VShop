using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Billing.API.Infrastructure.IntegrationMigrations
{
    public partial class ChangedDatetimeColumnsTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "integration",
                table: "integration_event_log");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "integration",
                table: "integration_event_log");

            migrationBuilder.AddColumn<Instant>(
                name: "date_created",
                schema: "integration",
                table: "integration_event_log",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_updated",
                schema: "integration",
                table: "integration_event_log",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "integration",
                table: "integration_event_log");

            migrationBuilder.DropColumn(
                name: "date_updated",
                schema: "integration",
                table: "integration_event_log");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "integration",
                table: "integration_event_log",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "integration",
                table: "integration_event_log",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
