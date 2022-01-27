using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.DAL.Migrations.Integration
{
    public partial class RenamedTableAndColumns : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_integration_event_log",
                schema: "integration",
                table: "integration_event_log");

            migrationBuilder.RenameTable(
                name: "integration_event_log",
                schema: "integration",
                newName: "integration_event_queue",
                newSchema: "integration");

            migrationBuilder.RenameColumn(
                name: "event_type_name",
                schema: "integration",
                table: "integration_event_queue",
                newName: "type_name");

            migrationBuilder.RenameColumn(
                name: "content",
                schema: "integration",
                table: "integration_event_queue",
                newName: "body");

            migrationBuilder.RenameColumn(
                name: "event_id",
                schema: "integration",
                table: "integration_event_queue",
                newName: "id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_integration_event_queue",
                schema: "integration",
                table: "integration_event_queue",
                column: "id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "pk_integration_event_queue",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.RenameTable(
                name: "integration_event_queue",
                schema: "integration",
                newName: "integration_event_log",
                newSchema: "integration");

            migrationBuilder.RenameColumn(
                name: "type_name",
                schema: "integration",
                table: "integration_event_log",
                newName: "event_type_name");

            migrationBuilder.RenameColumn(
                name: "body",
                schema: "integration",
                table: "integration_event_log",
                newName: "content");

            migrationBuilder.RenameColumn(
                name: "id",
                schema: "integration",
                table: "integration_event_log",
                newName: "event_id");

            migrationBuilder.AddPrimaryKey(
                name: "pk_integration_event_log",
                schema: "integration",
                table: "integration_event_log",
                column: "event_id");
        }
    }
}
