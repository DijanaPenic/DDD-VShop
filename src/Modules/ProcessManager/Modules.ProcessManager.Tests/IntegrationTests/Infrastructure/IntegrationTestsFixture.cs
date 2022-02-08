using Serilog;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Configuration;

using VShop.Modules.ProcessManager.Infrastructure.Configuration;
using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;
using VShop.SharedKernel.Tests.IntegrationTests;
using VShop.SharedKernel.Tests.IntegrationTests.Contracts;

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

            IContextAccessor contextAccessor = new MockContextAccessor();
            
            ILogger logger = new LoggerConfiguration()
                .Enrich.FromLogContext()
                .CreateLogger();
            
            IMemoryCache memoryCache = new MemoryCache(new MemoryCacheOptions());
            IMessageContextRegistry messageContextRegistry = new MessageContextRegistry(memoryCache);
            
            Module module = ModuleLoader.LoadModules(configuration).Single();
            module.ConfigureContainer(configuration, logger, contextAccessor, messageContextRegistry);

            ProcessManagerModule = new ModuleFixture
            (
                ProcessManagerCompositionRoot.ServiceProvider,
                configuration,
                module.Name
            );
        }
    }
}