using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.API.Infrastructure.SchedulerMigrations
{
    public partial class RenamedRuntimeTypeField : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "runtime_type",
                schema: "scheduler",
                table: "message_log",
                newName: "type_name");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "type_name",
                schema: "scheduler",
                table: "message_log",
                newName: "runtime_type");
        }
    }
}
