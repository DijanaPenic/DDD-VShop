using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Billing.API.Infrastructure.Migrations
{
    public partial class AddedErrorFieldToPaymentTable : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "error",
                schema: "payment",
                table: "payment_transfer",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "error",
                schema: "payment",
                table: "payment_transfer");
        }
    }
}
