using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        private static readonly IConfiguration Configuration;
        public static string RelationalDbConnectionString => Configuration["Sales:Postgres:ConnectionString"];
        public static IModuleFixture SalesModule { get; }
        static IntegrationTestsFixture()
        {
            Configuration = new ConfigurationBuilder()
                .AddJsonFile("module.sales.tests.json")
                .Build();

            IContextAccessor contextAccessor = new MockContextAccessor();
            
            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();
            
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            IMessageContextRegistry messageContextRegistry = new MessageContextRegistry(memoryCache);
            
            IModule module = ModuleLoader.LoadModules(Configuration).Single();
            module.ConfigureCompositionRoot(Configuration, logger, contextAccessor, messageContextRegistry);

            SalesModule = new ModuleFixture(SalesCompositionRoot.ServiceProvider);
            SalesModule.InitializePostgresDatabaseAsync().GetAwaiter().GetResult();
        }
    }
}