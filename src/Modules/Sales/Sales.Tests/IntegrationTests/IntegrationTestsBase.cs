using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac.Extensions.DependencyInjection;
using Dapper;
using Npgsql;
using Serilog;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;

using VShop.Modules.Sales.API;
using VShop.Modules.Sales.Infrastructure;
using VShop.SharedKernel.Scheduler.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;

namespace VShop.Modules.Sales.Tests.IntegrationTests
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
            await RestartEventStoreDatabaseAsync();
        }

        // Source: https://github.com/EventStore/EventStore/issues/1328
        // Restart will also clear the EventStore database (in-mem mode is used).
        private async Task RestartEventStoreDatabaseAsync()
        {
            HttpClient client = new();

            HttpResponseMessage result = await client.PostAsync($"{_configuration["EventStoreDbPortalUrl"]}/admin/shutdown", null);

            if (!result.IsSuccessStatusCode) throw new Exception("Event Store database restart failed.");
        }
        
        private Task RunPostgresDatabaseMigrationsAsync()
        {
            using SalesContext salesContext = _scope.ServiceProvider.GetService<SalesContext>();
            salesContext?.Database.Migrate();
            
            using SchedulerContext schedulerContext = _scope.ServiceProvider.GetService<SchedulerContext>();
            schedulerContext?.Database.Migrate();
            
            using SubscriptionContext subscriptionContext = _scope.ServiceProvider.GetService<SubscriptionContext>();
            subscriptionContext?.Database.Migrate();
            
            return Task.CompletedTask;
        }
        
        private async Task ClearPostgresDatabaseAsync()
        {
            await using NpgsqlConnection connection = new(_configuration.GetConnectionString("PostgresDb"));
            
            await connection.OpenAsync();

            const string sql = @"DELETE FROM ""scheduler"".""qrtz_job_details""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_simple_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_simprop_triggers""; " +
                               @"DELETE FROM ""scheduler"".""qrtz_cron_triggers""; " +
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