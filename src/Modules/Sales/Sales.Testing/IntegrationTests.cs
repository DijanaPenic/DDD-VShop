using Moq;
using System;
using System.Net.Mail;
using Autofac;
using AutoFixture;
using EventStore.Client;

using VShop.Modules.Sales.Domain.Services;
using VShop.Modules.Sales.Infrastructure.Services;
using VShop.Modules.Sales.API.Infrastructure.AutofacModules;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Scheduler.Quartz.Services;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.Repositories.Contracts;

namespace VShop.Modules.Sales.Testing
{
    public abstract class IntegrationTests
    {
        protected readonly Fixture Fixture;
        protected readonly IContainer Container;

        protected IntegrationTests()
        {
            // Configure container builder
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

            // Build container
            Container = builder.Build();

            // Fixture configuration
            Fixture = new Fixture();
            Fixture.Register(() => EmailAddress.Create(Fixture.Create<MailAddress>().Address));
            Fixture.Register(() => PhoneNumber.Create("+385929551178"));
            Fixture.Register(() => FullName.Create
            (
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>()
            ));
            Fixture.Register(() => Address.Create
            (
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>(),
                Fixture.Create<string>()
            ));
            Fixture.Register(() => EntityId.Create(Fixture.Create<Guid>()));
            
            // TODO - add type mapper for better ES preview
        }
    }
    
    public static class FixtureExtensions // TODO - move to a new class
    {
        public static int CreateInt(this IFixture fixture, int min, int max) 
            => fixture.Create<int>() % (max - min + 1) + min;
    }
}