using System;
using Microsoft.EntityFrameworkCore.Migrations;
using NodaTime;

#nullable disable

namespace VShop.Modules.Identity.Infrastructure.DAL.Migrations.Identity
{
    public partial class SeededRoleClaims : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                schema: "identity",
                table: "role_claim",
                columns: new[] { "id", "claim_type", "claim_value", "date_created", "date_updated", "role_id" },
                values: new object[,]
                {
                    { new Guid("63ed58ac-924b-551d-99f5-4de7a949db97"), "permission", "shopping_carts", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("688d85e7-178b-5a6f-bce8-753f331af68b"), "permission", "auth", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("860cb18e-717c-58b4-808f-04f84a548a6f"), "permission", "categories", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("a368b420-2aef-532a-a413-93089dd1fa75"), "permission", "orders", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c33b6d82-374e-5acd-a20a-56115f3efd1a"), "permission", "products", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c86fcd6d-8d01-5a0c-9860-6f7d4e9e6cf6"), "permission", "payments", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("c8b65e13-1730-5033-8074-e14d0a7b88ea"), "permission", "categories", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d92ef5e5-f08a-4173-b83a-74618893891b") },
                    { new Guid("e11969e7-6f99-514d-9ab4-169b842258e0"), "permission", "products", NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), NodaTime.Instant.FromUnixTimeTicks(16409952000000000L), new Guid("d72ef5e5-f08a-4173-b83a-74618893891b") }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("63ed58ac-924b-551d-99f5-4de7a949db97"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("688d85e7-178b-5a6f-bce8-753f331af68b"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("860cb18e-717c-58b4-808f-04f84a548a6f"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("a368b420-2aef-532a-a413-93089dd1fa75"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c33b6d82-374e-5acd-a20a-56115f3efd1a"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c86fcd6d-8d01-5a0c-9860-6f7d4e9e6cf6"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("c8b65e13-1730-5033-8074-e14d0a7b88ea"));

            migrationBuilder.DeleteData(
                schema: "identity",
                table: "role_claim",
                keyColumn: "id",
                keyValue: new Guid("e11969e7-6f99-514d-9ab4-169b842258e0"));
        }
    }
}
