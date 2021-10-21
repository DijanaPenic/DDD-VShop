using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Services.Basket.API.Infrastructure.Migrations
{
    public partial class AddedTimestampFields : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "basket",
                table: "basket_details_product_item",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "basket",
                table: "basket_details_product_item",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_created_utc",
                schema: "basket",
                table: "basket_details",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "date_updated_utc",
                schema: "basket",
                table: "basket_details",
                type: "timestamp without time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "basket",
                table: "basket_details_product_item");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "basket",
                table: "basket_details_product_item");

            migrationBuilder.DropColumn(
                name: "date_created_utc",
                schema: "basket",
                table: "basket_details");

            migrationBuilder.DropColumn(
                name: "date_updated_utc",
                schema: "basket",
                table: "basket_details");
        }
    }
}
