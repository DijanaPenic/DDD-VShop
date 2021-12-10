using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Billing.API.Infrastructure.Migrations
{
    public partial class ChangedDatetimeColumnsTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.AddColumn<Instant>(
                name: "date_created",
                schema: "payment",
                table: "payment_transfer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_updated",
                schema: "payment",
                table: "payment_transfer",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.DropColumn(
                name: "date_updated",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "payment",
                table: "payment_transfer",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "payment",
                table: "payment_transfer",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
