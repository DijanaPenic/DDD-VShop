using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Identity.Infrastructure.DAL.Migrations.Identity
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "identity");

            migrationBuilder.CreateTable(
                name: "client",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    secret = table.Column<string>(type: "text", nullable: false),
                    description = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    active = table.Column<bool>(type: "boolean", nullable: false),
                    access_token_life_time = table.Column<int>(type: "integer", nullable: false),
                    refresh_token_life_time = table.Column<int>(type: "integer", nullable: false),
                    allowed_origin = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_client", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    stackable = table.Column<bool>(type: "boolean", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "user",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    normalized_user_name = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    email_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    normalized_email = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    phone_number = table.Column<string>(type: "text", nullable: true),
                    phone_number_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    password_hash = table.Column<string>(type: "text", nullable: true),
                    two_factor_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    lockout_enabled = table.Column<bool>(type: "boolean", nullable: false),
                    is_approved = table.Column<bool>(type: "boolean", nullable: false),
                    access_failed_count = table.Column<int>(type: "integer", nullable: false),
                    security_stamp = table.Column<string>(type: "text", nullable: true),
                    lockout_end_date = table.Column<Instant>(type: "timestamp with time zone", nullable: true),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "role_claim",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_role_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_role_claim_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_claim",
                schema: "identity",
                columns: table => new
                {
                    id = table.Column<Guid>(type: "uuid", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    claim_type = table.Column<string>(type: "text", nullable: false),
                    claim_value = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_claim", x => x.id);
                    table.ForeignKey(
                        name: "fk_user_claim_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_login",
                schema: "identity",
                columns: table => new
                {
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_key = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    provider_display_name = table.Column<string>(type: "text", nullable: false),
                    token = table.Column<string>(type: "character varying(300)", maxLength: 300, nullable: true),
                    is_confirmed = table.Column<bool>(type: "boolean", nullable: false),
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_login", x => new { x.login_provider, x.provider_key });
                    table.ForeignKey(
                        name: "fk_user_login_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_refresh_token",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    client_id = table.Column<Guid>(type: "uuid", nullable: false),
                    value = table.Column<string>(type: "character varying(256)", maxLength: 256, nullable: false),
                    date_expires = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_refresh_token", x => new { x.user_id, x.client_id });
                    table.ForeignKey(
                        name: "fk_user_refresh_token_client_client_id",
                        column: x => x.client_id,
                        principalSchema: "identity",
                        principalTable: "client",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_refresh_token_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_role",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    role_id = table.Column<Guid>(type: "uuid", nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_role", x => new { x.role_id, x.user_id });
                    table.ForeignKey(
                        name: "fk_user_role_roles_role_id",
                        column: x => x.role_id,
                        principalSchema: "identity",
                        principalTable: "role",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "fk_user_role_users_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "user_token",
                schema: "identity",
                columns: table => new
                {
                    user_id = table.Column<Guid>(type: "uuid", nullable: false),
                    login_provider = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    name = table.Column<string>(type: "character varying(128)", maxLength: 128, nullable: false),
                    value = table.Column<string>(type: "text", nullable: false),
                    xmin = table.Column<uint>(type: "xid", rowVersion: true, nullable: false),
                    date_created = table.Column<Instant>(type: "timestamp with time zone", nullable: false),
                    date_updated = table.Column<Instant>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_user_token", x => new { x.user_id, x.login_provider, x.name });
                    table.ForeignKey(
                        name: "fk_user_token_user_user_id",
                        column: x => x.user_id,
                        principalSchema: "identity",
                        principalTable: "user",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "client",
                columns: new[] { "id", "access_token_life_time", "active", "allowed_origin", "date_created", "date_updated", "description", "name", "refresh_token_life_time", "secret" },
                values: new object[] { new Guid("5c52160a-4ab4-49c6-ba5f-56df9c5730b6"), 20, true, "*", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), "Web API Application", "WebApiApplication", 60, "PX23zsV/7nm6+ZI9LmrKXSBf2O47cYtiJGk2WJ/G/PdU2eD7Y929MZeItkGpBY2v6a2tXhGINq8bAQYz1bQC6A==" });

            migrationBuilder.InsertData(
                schema: "identity",
                table: "role",
                columns: new[] { "id", "date_created", "date_updated", "name", "normalized_name", "stackable" },
                values: new object[,]
                {
                    { new Guid("9621c09c-06b1-45fb-8baf-38e0757e2f59"), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), "Guest", "GUEST", false },
                    { new Guid("d72ef5e5-f08a-4173-b83a-74618893891b"), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), "Admin", "ADMIN", false },
                    { new Guid("d82ef5e5-f08a-4173-b83a-74618893891b"), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), "Customer", "CUSTOMER", true },
                    { new Guid("d92ef5e5-f08a-4173-b83a-74618893891b"), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), "Store Manager", "STORE MANAGER", true }
                });

            migrationBuilder.CreateIndex(
                name: "NameIndex",
                schema: "identity",
                table: "client",
                column: "name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "RoleNameIndex",
                schema: "identity",
                table: "role",
                column: "normalized_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_role_claim_role_id",
                schema: "identity",
                table: "role_claim",
                column: "role_id");

            migrationBuilder.CreateIndex(
                name: "EmailIndex",
                schema: "identity",
                table: "user",
                column: "normalized_email");

            migrationBuilder.CreateIndex(
                name: "UserNameIndex",
                schema: "identity",
                table: "user",
                column: "normalized_user_name",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "ix_user_claim_user_id",
                schema: "identity",
                table: "user_claim",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_login_user_id",
                schema: "identity",
                table: "user_login",
                column: "user_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_refresh_token_client_id",
                schema: "identity",
                table: "user_refresh_token",
                column: "client_id");

            migrationBuilder.CreateIndex(
                name: "ix_user_role_user_id",
                schema: "identity",
                table: "user_role",
                column: "user_id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "role_claim",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user_claim",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user_login",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user_refresh_token",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user_role",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user_token",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "client",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "role",
                schema: "identity");

            migrationBuilder.DropTable(
                name: "user",
                schema: "identity");
        }
    }
}
