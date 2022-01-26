using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Billing.Infrastructure.DAL.Migrations.Integration
{
    public partial class EnabledXminConcurrenyToken : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<uint>(
                name: "xmin",
                schema: "integration",
                table: "integration_event_log",
                type: "xid",
                rowVersion: true,
                nullable: false,
                defaultValue: 0u);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "xmin",
                schema: "integration",
                table: "integration_event_log");
        }
    }
}
