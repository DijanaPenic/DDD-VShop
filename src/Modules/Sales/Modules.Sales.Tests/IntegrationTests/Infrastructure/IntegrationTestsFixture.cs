using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;
using VShop.Modules.Sales.Infrastructure.Configuration;

namespace VShop.Modules.Sales.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        public static IModuleFixture SalesModule { get; }

        static IntegrationTestsFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("module.sales.tests.json")
                .Build();
            
            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .ReadFrom.Configuration(configuration)
                .CreateLogger();
            
            Module module = ModuleLoader.LoadModules(configuration).Single();
            
            IServiceCollection services = new ServiceCollection();
            services.AddSingleton(configuration);
            services.AddContext(new MockContextAccessor());
            services.AddMessaging();
            services.AddAuth(module);

            module.ConfigureContainer(logger, configuration, services);

            SalesModule = new ModuleFixture
            (
                SalesCompositionRoot.ServiceProvider,
                configuration,
                module.Name
            );
        }
    }
}