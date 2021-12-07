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
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Testing
{
    public abstract class IntegrationTests
    {
        protected readonly Fixture Fixture;
        protected readonly IContainer Container;

        protected IntegrationTests()
        {
            // Container setup
            Container = InitializeTestContainer();

            // Fixture configuration
            Fixture = InitializeTestFixtures();
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
            
        private static Fixture InitializeTestFixtures()
        {
            // TODO - maybe some helper class?
            Fixture fixture = new();
            
            fixture.Register(() => EmailAddress.Create(fixture.Create<MailAddress>().Address));
            fixture.Register(() => PhoneNumber.Create("+385929551178"));
            fixture.Register(() => FullName.Create
            (
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            ));
            fixture.Register(() => Address.Create
            (
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>(),
                fixture.Create<string>()
            ));
            fixture.Register(() => EntityId.Create(fixture.Create<Guid>()));
            fixture.Register(() => ProductQuantity.Create(fixture.CreateInt(0, 10)));
            fixture.Register(() => Price.Create(fixture.CreateDecimal(10, 100)));

            return fixture;
        }
    }
}