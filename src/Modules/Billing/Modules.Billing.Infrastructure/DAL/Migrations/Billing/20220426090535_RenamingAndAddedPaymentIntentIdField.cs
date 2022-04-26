using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.DAL.Migrations.Billing
{
    public partial class RenamingAndAddedPaymentIntentIdField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "intent_id",
                schema: "payment",
                table: "payment",
                type: "text",
                nullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "intent_id",
                schema: "payment",
                table: "payment");
        }
    }
}
