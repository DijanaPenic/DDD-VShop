using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Microsoft.EntityFrameworkCore;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests.Infrastructure
{
    public class ResetDatabaseLifetime : IAsyncLifetime
    {
        public async Task InitializeAsync()
        {
            // Postgres database
            await RunRelationalDatabaseMigrationsAsync();
            await ClearRelationalDatabaseAsync();
            
            // EventStore database
            //await RestartEventStoreDatabaseAsync();
        }
        
        private static async Task RunRelationalDatabaseMigrationsAsync()
        {
            await MigratePostgresDatabaseAsync<SalesContext>();
            await MigratePostgresDatabaseAsync<SchedulerContext>();
            await MigratePostgresDatabaseAsync<SubscriptionContext>();
        }
        
        private static Task MigratePostgresDatabaseAsync<TDbContext>() 
            where TDbContext : DbContextBase
             => IntegrationTestsFixture.ExecuteServiceAsync<TDbContext>(dbContext => dbContext.Database.MigrateAsync());

        private static async Task ClearRelationalDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(IntegrationTestsFixture.RelationalDbConnectionString);
            
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""scheduler"".""message_log""; " +
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
        
        // Source: https://github.com/EventStore/EventStore/issues/1328
        // Restart will also clear the EventStore database (in-mem mode is used).
        private static async Task RestartEventStoreDatabaseAsync()
        {
            try
            {
                // TODO - I'm randomly getting the following error:
                // Status(StatusCode="Internal", Detail="Error starting gRPC call. HttpRequestException: An error occurred
                // while sending the request. IOException: The request was aborted. IOException: An HTTP/2 connection could
                // not be established because the server did not complete the HTTP/2 handshake. IOException: The response
                // ended prematurely while waiting for the next frame from the server.", DebugException="System.Net.Http
                HttpClient client = new();

                HttpResponseMessage result = await client
                    .PostAsync($"{IntegrationTestsFixture.EventStorePortalUrl}/admin/shutdown", null);

                if (!result.IsSuccessStatusCode) throw new Exception("Event Store database restart failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        public Task DisposeAsync() => Task.CompletedTask;
    }
}