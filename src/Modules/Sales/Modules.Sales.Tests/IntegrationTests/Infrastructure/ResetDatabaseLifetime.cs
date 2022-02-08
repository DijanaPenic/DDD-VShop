using Npgsql;
using Dapper;

using VShop.SharedKernel.Tests.IntegrationTests;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    public class ResetDatabaseLifetime : DatabaseLifetime
    {
        protected override async Task ClearRelationalDatabaseAsync() 
        {
            await using NpgsqlConnection connection = new(IntegrationTestsFixture.SalesModule.RelationalDbConnectionString);
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""shopping_cart"".""shopping_cart_info_product_item""; " +
                               @"DELETE FROM ""shopping_cart"".""shopping_cart_info""; " +
                               @"DELETE FROM ""subscription"".""checkpoint""; ";

            await connection.ExecuteScalarAsync(sql);
            await connection.CloseAsync();
        }
    }
}