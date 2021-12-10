using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.SubscriptionMigrations
{
    public partial class ChangedDatetimeColumnsTypeAndName : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_updated_utc",
                schema: "subscription",
                table: "checkpoint",
                newName: "date_updated");

            migrationBuilder.RenameColumn(
                name: "date_created_utc",
                schema: "subscription",
                table: "checkpoint",
                newName: "date_created");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "date_updated",
                schema: "subscription",
                table: "checkpoint",
                newName: "date_updated_utc");

            migrationBuilder.RenameColumn(
                name: "date_created",
                schema: "subscription",
                table: "checkpoint",
                newName: "date_created_utc");
        }
    }
}
