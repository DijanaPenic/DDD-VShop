using Moq;
using Autofac;
using EventStore.Client;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;
using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    public abstract class IntegrationTestsBase
    {
        protected readonly IContainer Container;

        protected IntegrationTestsBase()
        {
            // Container setup
            Container = InitializeTestContainer();
        }

        private static IContainer InitializeTestContainer()
        {
            ContainerBuilder builder = new();
            
            builder.RegisterModule<MediatorModule>();
            builder.RegisterGeneric(typeof(EventStoreProcessManagerRepository<>))
                .As(typeof(IProcessManagerRepository<>)).SingleInstance();
            builder.RegisterGeneric(typeof(EventStoreAggregateRepository<,>))
                .As(typeof(IAggregateRepository<,>)).SingleInstance();
            
            // TODO - need to fix as this is pointing to the existing database
            // TODO - move configuration into the settings file
            const string dbConnectionString = "esdb://admin:changeit@localhost:2113?tls=false&tlsVerifyCert=false";
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(dbConnectionString);
            eventStoreSettings.ConnectionName = "SalesIntegrationTests";
            EventStoreClient eventStoreClient = new(eventStoreSettings);
            builder.Register(_=> eventStoreClient).SingleInstance(); // TODO - single instance

            Mock<ISchedulerService> schedulerServiceMock = new();
            builder.Register(_=> schedulerServiceMock.Object);

            builder.RegisterType<ShoppingCartOrderingService>().As<IShoppingCartOrderingService>();
            
            return builder.Build();
        }
    }
}