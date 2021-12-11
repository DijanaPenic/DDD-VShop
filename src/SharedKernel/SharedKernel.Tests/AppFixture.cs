using System;
using System.Net.Mail;
using AutoFixture;

using VShop.SharedKernel.Tests.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Tests
{
    public class AppFixture
    {
        public Fixture AutoFixture { get; }

        public AppFixture()
        {
            AutoFixture = new Fixture();
            
            AutoFixture.Register(() => EmailAddress.Create(AutoFixture.Create<MailAddress>().Address));
            AutoFixture.Register(() => PhoneNumber.Create("+385929551178"));
            AutoFixture.Register(() => FullName.Create
            (
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>()
            ));
            AutoFixture.Register(() => Address.Create
            (
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>(),
                AutoFixture.Create<string>()
            ));
            AutoFixture.Register(() => EntityId.Create(AutoFixture.Create<Guid>()));
            AutoFixture.Register(() => ProductQuantity.Create(AutoFixture.CreateInt(1, 10)));
            AutoFixture.Register(() => Price.Create(AutoFixture.CreateDecimal(10, 100)));
            AutoFixture.Register(() => Discount.Create(AutoFixture.CreateInt(0, 100)));
        } 
    }
}