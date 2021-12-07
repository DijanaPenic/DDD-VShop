using Moq;
using Autofac;
using EventStore.Client;

using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.Modules.Sales.Tests.IntegrationTests
{
    internal static class ContainerBuilderExtensions
    {
        internal static void RegisterEventStore(this ContainerBuilder builder, string connectionString)
        {
            EventStoreClientSettings eventStoreSettings = EventStoreClientSettings.Create(connectionString);
            eventStoreSettings.ConnectionName = "SalesTests";

            builder.Register(_=> new EventStoreClient(eventStoreSettings)).SingleInstance();
            
            builder.RegisterGeneric(typeof(EventStoreProcessManagerRepository<>))
                .As(typeof(IProcessManagerRepository<>)).SingleInstance();
            builder.RegisterGeneric(typeof(EventStoreAggregateRepository<,>))
                .As(typeof(IAggregateRepository<,>)).SingleInstance();
        }
        
        internal static void RegisterScheduler(this ContainerBuilder builder)
        {
            Mock<ISchedulerService> schedulerServiceMock = new();
            builder.Register(_=> schedulerServiceMock.Object); // TODO - need to configure
        }
    }
}