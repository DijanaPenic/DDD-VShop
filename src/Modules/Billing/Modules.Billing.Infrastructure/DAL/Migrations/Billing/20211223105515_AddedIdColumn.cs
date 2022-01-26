using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.DAL.Migrations.Billing
{
    public partial class AddedIdColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_payment_transfer",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.AddColumn<Guid>(
                name: "id",
                schema: "payment",
                table: "payment_transfer",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment_transfer",
                schema: "payment",
                table: "payment_transfer",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_payment_transfer",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.DropColumn(
                name: "id",
                schema: "payment",
                table: "payment_transfer");

            migrationBuilder.AddPrimaryKey(
                name: "pk_payment_transfer",
                schema: "payment",
                table: "payment_transfer",
                column: "order_id");
        }
    }
}
