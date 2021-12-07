using System;
using System.Net.Http;
using Autofac;
using Microsoft.Extensions.Configuration;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        private readonly IConfiguration _configuration;
        
        protected readonly IContainer Container;

        protected IntegrationTestsBase()
        {
            // Init configuration
            _configuration = InitConfiguration();
            
            // Container setup
            Container = InitializeDependencyInjectionContainer();
            
            // Restart EventStore database
            RestartEventStoreDatabase();
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

        // Source: https://github.com/EventStore/EventStore/issues/1328
        private void RestartEventStoreDatabase()
        {
            HttpClient client = new();
            
            HttpResponseMessage result = client.PostAsync($"{_configuration["EventStoreDb:AppUrl"]}/admin/shutdown", null)
                .GetAwaiter().GetResult();

            if (!result.IsSuccessStatusCode) throw new Exception("Event Store database restart failed.");
        }
        
        private static IConfiguration InitConfiguration()
            => new ConfigurationBuilder()
                .AddJsonFile("appsettings.Test.json")
                .AddEnvironmentVariables() 
                .Build();
    }
}