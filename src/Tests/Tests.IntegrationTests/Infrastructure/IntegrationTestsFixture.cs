using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Extensions;

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

            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();

            Module[] modules = ModuleLoader.LoadModules(configuration).ToArray();
            
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddContext(new MockContextAccessor());
            services.AddMessaging();

            foreach (Module module in modules)
                module.ConfigureContainer(logger, configuration, services.Clone());

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