using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

namespace VShop.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        public static IModuleFixture SalesModule { get; }
        public static IModuleFixture ProcessManagerModule { get; }
        public static IModuleFixture BillingModule { get; }
        public static IModuleFixture CatalogModule { get; }

        static IntegrationTestsFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("tests.json")
                .Build();

            IContextAccessor contextAccessor = new MockContextAccessor();
            
            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();
            
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            IMessageContextRegistry messageContextRegistry = new MessageContextRegistry(memoryCache);
            
            IList<IModule> modules = ModuleLoader.LoadModules(configuration);
            foreach (IModule module in modules)
                module.ConfigureCompositionRoot(configuration, logger, contextAccessor, messageContextRegistry);

            SalesModule = new ModuleFixture
            (
                SalesCompositionRoot.ServiceProvider,
                configuration,
                "Sales"
            );
            ProcessManagerModule = new ModuleFixture
            (
                ProcessManagerCompositionRoot.ServiceProvider,
                configuration,
                "ProcessManager"
            );
            BillingModule = new ModuleFixture
            (
                BillingCompositionRoot.ServiceProvider,
                configuration,
                "Billing"
            );
            CatalogModule = new ModuleFixture
            (
                CatalogCompositionRoot.ServiceProvider,
                configuration,
                "Catalog"
            );
        }
    }
}