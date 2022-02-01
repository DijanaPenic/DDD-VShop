using Npgsql;
using Dapper;

using VShop.SharedKernel.Tests.IntegrationTests;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    public abstract class ResetDatabaseLifetime : DatabaseLifetime
    {
        protected override Task ClearRelationalDatabaseAsync() => ResetRelationalDatabaseAsync();

        public static async Task ResetRelationalDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(IntegrationTestsFixture.RelationalDbConnectionString);
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""shopping_cart"".""shopping_cart_info_product_item""; " +
                               @"DELETE FROM ""shopping_cart"".""shopping_cart_info""; " +
                               @"DELETE FROM ""subscription"".""checkpoint""; ";

            await connection.ExecuteScalarAsync(sql);
            await connection.CloseAsync();
        }
    }
}