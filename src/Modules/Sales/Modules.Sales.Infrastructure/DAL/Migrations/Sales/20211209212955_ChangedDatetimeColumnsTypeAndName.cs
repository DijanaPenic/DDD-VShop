using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Sales
{
    public partial class ChangedDatetimeColumnsTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info");

            migrationBuilder.AddColumn<Instant>(
                name: "date_created",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_updated",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_created",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));

            migrationBuilder.AddColumn<Instant>(
                name: "date_updated",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: NodaTime.Instant.FromUnixTimeTicks(0L));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "date_updated",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item");

            migrationBuilder.DropColumn(
                name: "date_created",
                schema: "shopping_cart",
                table: "shopping_cart_info");

            migrationBuilder.DropColumn(
                name: "date_updated",
                schema: "shopping_cart",
                table: "shopping_cart_info");

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info_product_item",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "shopping_cart",
                table: "shopping_cart_info",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
