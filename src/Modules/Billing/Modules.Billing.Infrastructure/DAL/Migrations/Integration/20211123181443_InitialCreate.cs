using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Billing.Infrastructure.DAL.Migrations.Integration
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "integration");

            migrationBuilder.CreateTable(
                name: "integration_event_log",
                schema: "integration",
                columns: table => new
                {
                    event_id = table.Column<Guid>(type: "uuid", nullable: false),
                    event_type_name = table.Column<string>(type: "text", nullable: false),
                    state = table.Column<int>(type: "integer", nullable: false),
                    times_sent = table.Column<int>(type: "integer", nullable: false),
                    content = table.Column<string>(type: "text", nullable: false),
                    transaction_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_integration_event_log", x => x.event_id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "integration_event_log",
                schema: "integration");
        }
    }
}
