using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace VShop.Modules.Sales.Infrastructure.DAL.Migrations.Scheduler
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "scheduler");

            migrationBuilder.CreateTable(
                name: "message_log",
                schema: "scheduler",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    body = table.Column<string>(type: "text", nullable: false),
                    runtime_type = table.Column<string>(type: "text", nullable: false),
                    message_type = table.Column<int>(type: "integer", nullable: false),
                    scheduled_time = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    status = table.Column<int>(type: "integer", nullable: false),
                    date_created_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false),
                    date_updated_utc = table.Column<DateTime>(type: "timestamp without time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_message_log", x => x.id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "message_log",
                schema: "scheduler");
        }
    }
}
