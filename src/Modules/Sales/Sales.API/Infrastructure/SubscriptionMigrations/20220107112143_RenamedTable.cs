﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.SubscriptionMigrations
{
    public partial class RenamedTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_message_dead_letter_log",
                schema: "subscription",
                table: "message_dead_letter_log");

            migrationBuilder.RenameTable(
                name: "message_dead_letter_log",
                schema: "subscription",
                newName: "message_dead_letter_queue",
                newSchema: "subscription");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message_dead_letter_queue",
                schema: "subscription",
                table: "message_dead_letter_queue",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_message_dead_letter_queue",
                schema: "subscription",
                table: "message_dead_letter_queue");

            migrationBuilder.RenameTable(
                name: "message_dead_letter_queue",
                schema: "subscription",
                newName: "message_dead_letter_log",
                newSchema: "subscription");

            migrationBuilder.AddPrimaryKey(
                name: "pk_message_dead_letter_log",
                schema: "subscription",
                table: "message_dead_letter_log",
                column: "id");
        }
    }
}
