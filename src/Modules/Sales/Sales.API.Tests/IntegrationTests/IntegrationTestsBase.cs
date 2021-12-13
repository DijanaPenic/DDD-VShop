using Xunit;
using System;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Dapper;
using Npgsql;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using Autofac.Extensions.DependencyInjection;

using VShop.SharedKernel.PostgresDb;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.Modules.Sales.Infrastructure;

namespace VShop.Modules.Sales.API.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase : IAsyncLifetime
    {
        private readonly IConfiguration _configuration;
        private IServiceScope _scope;

        protected IntegrationTestsBase()
        {
            IHost host = Host.CreateDefaultBuilder()
                .ConfigureAppConfiguration((_, config) =>
                {
                    config.AddJsonFile("appsettings.Test.json");
                })
                .UseSerilog((context, services, configuration) => configuration
                    .ReadFrom.Configuration(context.Configuration)
                    .ReadFrom.Services(services)
                    .Enrich.FromLogContext()
                )
                .UseServiceProviderFactory(new AutofacServiceProviderFactory())
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    webBuilder.UseStartup<Startup>();
                })
                .Build();

            _scope = host.Services.GetRequiredService<IServiceScopeFactory>().CreateScope();
            _configuration = host.Services.GetRequiredService<IConfiguration>();
        }

        protected TService GetService<TService>() => _scope.ServiceProvider.GetService<TService>();

        public async Task InitializeAsync()
        {
            // Postgres database
            await RunPostgresDatabaseMigrationsAsync();
            await ClearPostgresDatabaseAsync();
            
            // EventStore database
            //await RestartEventStoreDatabaseAsync();
        }

        // Source: https://github.com/EventStore/EventStore/issues/1328
        // Restart will also clear the EventStore database (in-mem mode is used).
        private async Task RestartEventStoreDatabaseAsync()
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
                    .PostAsync($"{_configuration["EventStoreDbPortalUrl"]}/admin/shutdown", null);

                if (!result.IsSuccessStatusCode) throw new Exception("Event Store database restart failed.");
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
        
        private Task RunPostgresDatabaseMigrationsAsync()
        {
            MigrateDatabase<SalesContext>();
            MigrateDatabase<SchedulerContext>();
            MigrateDatabase<SubscriptionContext>();
            
            return Task.CompletedTask;
        }

        private void MigrateDatabase<TDbContext>() where TDbContext : DbContextBase
        {
            TDbContext dbContext = GetService<TDbContext>();
            dbContext.Database.Migrate();
        }
        
        private async Task ClearPostgresDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(_configuration.GetConnectionString("PostgresDb"));
            
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

        public Task DisposeAsync()
        {
            _scope?.Dispose();
            _scope = null;

            return Task.CompletedTask;
        }
    }
}