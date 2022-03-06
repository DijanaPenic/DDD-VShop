using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace VShop.Modules.Identity.Infrastructure.DAL.Migrations.Identity
{
    public partial class ChangedIndexNamesAndAddedIdentityTimestampRequirements : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "user",
                newName: "user_name_index");

            migrationBuilder.RenameIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "user",
                newName: "user_email_index");

            migrationBuilder.RenameIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "role",
                newName: "role_name_index");

            migrationBuilder.RenameIndex(
                name: "NameIndex",
                schema: "identity",
                table: "client",
                newName: "client_name_index");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameIndex(
                name: "user_name_index",
                schema: "identity",
                table: "user",
                newName: "UserNameIndex");

            migrationBuilder.RenameIndex(
                name: "user_email_index",
                schema: "identity",
                table: "user",
                newName: "EmailIndex");

            migrationBuilder.RenameIndex(
                name: "role_name_index",
                schema: "identity",
                table: "role",
                newName: "RoleNameIndex");

            migrationBuilder.RenameIndex(
                name: "client_name_index",
                schema: "identity",
                table: "client",
                newName: "NameIndex");
        }
    }
}
