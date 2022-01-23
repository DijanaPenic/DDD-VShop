using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Core.DAL.Migrations.Scheduler
{
    public partial class RemovedMessageTypeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "message_type",
                schema: "scheduler",
                table: "message_log");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "message_type",
                schema: "scheduler",
                table: "message_log",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }
    }
}
