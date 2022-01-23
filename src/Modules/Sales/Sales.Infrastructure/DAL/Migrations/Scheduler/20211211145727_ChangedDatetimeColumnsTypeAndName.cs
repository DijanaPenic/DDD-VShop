using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Scheduler
{
    public partial class ChangedDatetimeColumnsTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AlterColumn<Instant>(
                name: "scheduled_time",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp with time zone",
                nullable: false,
                oldClrType: typeof(DateTime),
                oldType: "timestamp without time zone");

            migrationBuilder.AddColumn<Instant>(
                name: "date_created",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_updated",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.DropColumn(
                name: "date_updated",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AlterColumn<DateTime>(
                name: "scheduled_time",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp without time zone",
                nullable: false,
                oldClrType: typeof(Instant),
                oldType: "timestamp with time zone");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "scheduler",
                table: "message_log",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
