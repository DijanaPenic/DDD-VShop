using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Scheduler
{
    public partial class AddedContextField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
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

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "context",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AddColumn<byte[]>(
                name: "metadata",
                schema: "scheduler",
                table: "message_log",
                type: "bytea",
                nullable: false,
                defaultValue: new byte[0]);
        }
    }
}
