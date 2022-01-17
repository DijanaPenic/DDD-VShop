using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.Migrations.Billing
{
    public partial class EnabledXminConcurrenyToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "payment",
                table: "payment_transfer",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "payment",
                table: "payment_transfer");
        }
    }
}
