using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Catalog.Infrastructure.DAL.Migrations.Integration
{
    public partial class UsedByteMessageRepresentation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "metadata",
                schema: "integration",
                table: "integration_event_queue",
                type: "bytea",
                nullable: false,
                defaultValue: Array.Empty<byte>());
            
            migrationBuilder.Sql("alter table integration.integration_event_queue alter column body type bytea using body::bytea");
            migrationBuilder.Sql("alter table integration.integration_event_queue alter column body set default \'\\x\'::bytea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                schema: "integration",
                table: "integration_event_queue");

            migrationBuilder.Sql("alter table integration.integration_event_queue alter column body type text using body::text");
            migrationBuilder.Sql("alter table integration.integration_event_queue alter column body type text drop default");
        }
    }
}
