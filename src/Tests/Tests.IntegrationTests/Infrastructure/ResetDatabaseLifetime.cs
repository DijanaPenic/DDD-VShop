using Dapper;
using Npgsql;

using VShop.SharedKernel.Tests.IntegrationTests;

namespace VShop.Tests.IntegrationTests.Infrastructure
{
    internal class ResetDatabaseLifetime : DatabaseLifetime
    {
        protected override Task ClearRelationalDatabaseAsync() => ResetRelationalDatabaseAsync();

        public static async Task ResetRelationalDatabaseAsync()
        {
            await using NpgsqlConnection salesConnection = new(IntegrationTestsFixture.SalesModule.RelationalDbConnectionString);
            await salesConnection.OpenAsync();

            const string salesSql = @"DELETE FROM ""shopping_cart"".""shopping_cart_info_product_item""; " +
                                    @"DELETE FROM ""shopping_cart"".""shopping_cart_info""; " +
                                    @"DELETE FROM ""subscription"".""checkpoint""; ";

            await salesConnection.ExecuteScalarAsync(salesSql);
            await salesConnection.CloseAsync();
            
            await using NpgsqlConnection processManagerConnection = new(IntegrationTestsFixture.ProcessManagerModule.RelationalDbConnectionString);
            await processManagerConnection.OpenAsync();

            const string processManagerSql = @"DELETE FROM ""subscription"".""checkpoint""; " +
                                             @"DELETE FROM ""scheduler"".""message_log""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_cron_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_simple_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_job_details""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_simprop_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_blob_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_calendars""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_paused_trigger_grps""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_fired_triggers""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_scheduler_state""; " +
                                             @"DELETE FROM ""scheduler"".""qrtz_locks""; ";

            await processManagerConnection.ExecuteScalarAsync(processManagerSql);
            await processManagerConnection.CloseAsync();
        }
    }
}