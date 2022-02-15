using Serilog;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Auth;
using VShop.SharedKernel.Infrastructure.Contexts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;
using VShop.Modules.ProcessManager.Infrastructure.Configuration;

namespace VShop.Modules.ProcessManager.Tests.IntegrationTests.Infrastructure
{
    internal static class IntegrationTestsFixture
    {
        public static IModuleFixture ProcessManagerModule { get; }
        
        static IntegrationTestsFixture()
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .AddJsonFile("module.process_manager.tests.json")
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

            ProcessManagerModule = new ModuleFixture
            (
                ProcessManagerCompositionRoot.ServiceProvider,
                configuration,
                module.Name
            );
        }
    }
}