using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.DAL.Migrations.Integration
{
    public partial class AddedContextField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.AddColumn<byte[]>(
                name: "metadata",
                schema: "integration",
                table: "integration_event_queue",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
