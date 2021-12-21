using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.SubscriptionMigrations
{
    public partial class EnabledXminConcurrenyToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "subscription",
                table: "checkpoint",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "subscription",
                table: "checkpoint");
        }
    }
}
