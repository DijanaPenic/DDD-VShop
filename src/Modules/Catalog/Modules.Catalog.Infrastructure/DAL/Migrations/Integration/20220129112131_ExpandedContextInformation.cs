using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.DAL.Migrations.Integration
{
    public partial class ExpandedContextInformation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.AddColumn<Guid>(
                name: "causation_id",
                schema: "integration",
                table: "integration_event_queue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "correlation_id",
                schema: "integration",
                table: "integration_event_queue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "user_id",
                schema: "integration",
                table: "integration_event_queue",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "causation_id",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.DropColumn(
                name: "correlation_id",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.DropColumn(
                name: "user_id",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.AddColumn<string>(
                name: "context",
                schema: "integration",
                table: "integration_event_queue",
                type: "text",
                nullable: false,
                defaultValue: "");
        }
    }
}
