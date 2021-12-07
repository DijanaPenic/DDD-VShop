using System;
using System.Net.Mail;
using AutoFixture;

using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.Modules.Sales.Testing
{
    public abstract class UnitTests
    {
        protected readonly Fixture Fixture;

        protected UnitTests()
        {
            // Fixture configuration
            Fixture = InitializeTestFixtures();
        }
        
        private static Fixture InitializeTestFixtures()
        {
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
            fixture.Register(() => ProductQuantity.Create(fixture.Create<int>()));
            fixture.Register(() => Price.Create(fixture.Create<decimal>()));

            return fixture;
        }
    }
}