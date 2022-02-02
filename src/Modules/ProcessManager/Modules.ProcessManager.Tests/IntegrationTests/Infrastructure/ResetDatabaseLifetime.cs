using Dapper;
using Npgsql;

using VShop.SharedKernel.Tests.IntegrationTests;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure
{
    internal abstract class ResetDatabaseLifetime : DatabaseLifetime
    {
        protected override Task ClearRelationalDatabaseAsync() => ResetRelationalDatabaseAsync();

        public static async Task ResetRelationalDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(IntegrationTestsFixture.ProcessManagerModule.RelationalDbConnectionString);
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""subscription"".""checkpoint""; " +
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

            await connection.ExecuteScalarAsync(sql);
            await connection.CloseAsync();
        }
    }
}