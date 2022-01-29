using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Scheduler
{
    public partial class ExpandedContextInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AddColumn<Guid>(
                name: "causation_id",
                schema: "scheduler",
                table: "message_log",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "correlation_id",
                schema: "scheduler",
                table: "message_log",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "scheduler",
                table: "message_log",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "causation_id",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.DropColumn(
                name: "correlation_id",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AddColumn<string>(
                name: "context",
                schema: "scheduler",
                table: "message_log",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
