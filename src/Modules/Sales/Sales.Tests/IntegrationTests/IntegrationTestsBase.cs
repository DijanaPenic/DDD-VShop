using Xunit;
using System;
using System.Net.Http;
using System.Threading.Tasks;
using Autofac;
using Microsoft.Extensions.Configuration;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    public class IntegrationTestsBase : IAsyncLifetime
    {
        private readonly IConfiguration _configuration;

        protected IContainer Container { get; }

        protected IntegrationTestsBase()
        {
            // Init configuration
            _configuration = InitializeConfiguration();
            
            // Container setup
            Container = InitializeDependencyInjectionContainer();
        }

        private IContainer InitializeDependencyInjectionContainer()
        {
            ContainerBuilder builder = new();
            
            builder.RegisterModule<MediatorModule>();
            builder.RegisterEventStore(_configuration["EventStoreDb:ConnectionString"]);
            builder.RegisterScheduler();
            builder.RegisterType<ShoppingCartOrderingService>().As<IShoppingCartOrderingService>();
            
            return builder.Build();
        }
        
        public Task InitializeAsync()
        {
            return RestartEventStoreDatabaseAsync();
        }

        // Source: https://github.com/EventStore/EventStore/issues/1328
        private async Task RestartEventStoreDatabaseAsync()
        {
            HttpClient client = new();

            HttpResponseMessage result = await client.PostAsync($"{_configuration["EventStoreDb:AppUrl"]}/admin/shutdown", null);

            if (!result.IsSuccessStatusCode) throw new Exception("Event Store database restart failed.");
        }
        
        private static IConfiguration InitializeConfiguration()
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables() 
                .Build();

        public Task DisposeAsync()
        {
            // ... clean up ...
            return Task.CompletedTask;
        }
    }
}