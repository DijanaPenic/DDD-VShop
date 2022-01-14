using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Sales.API.Infrastructure.SchedulerMigrations
{
    public partial class UsedByteMessageRepresentation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<byte[]>(
                name: "metadata",
                schema: "scheduler",
                table: "message_log",
                type: "bytea",
                nullable: false,
                defaultValue: Array.Empty<byte>());

            migrationBuilder.Sql("alter table scheduler.message_log alter column body type bytea using body::bytea");
            migrationBuilder.Sql("alter table scheduler.message_log alter column body set default \'\\x\'::bytea");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "metadata",
                schema: "scheduler",
                table: "message_log");

            migrationBuilder.AlterColumn<string>(
                name: "body",
                schema: "scheduler",
                table: "message_log",
                type: "text",
                nullable: false,
                oldClrType: typeof(byte[]),
                oldType: "bytea");
            
            migrationBuilder.Sql("alter table scheduler.message_log alter column body type text using body::text");
            migrationBuilder.Sql("alter table scheduler.message_log alter column body type text drop default");
        }
    }
}
