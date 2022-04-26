using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.DAL.Migrations.Billing
{
    public partial class AddedAmountField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "amount",
                schema: "payment",
                table: "payment",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "amount",
                schema: "payment",
                table: "payment");
        }
    }
}
