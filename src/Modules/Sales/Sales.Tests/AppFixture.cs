using System;
using System.Net.Mail;
using AutoFixture;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Tests
{
    internal static class AppFixture
    {
        public static readonly Fixture SalesFixture;

        static AppFixture()
        {
            SalesFixture = new Fixture();
            
            SalesFixture.Register(() => EmailAddress.Create(SalesFixture.Create<MailAddress>().Address));
            SalesFixture.Register(() => PhoneNumber.Create("+385929551178"));
            SalesFixture.Register(() => FullName.Create
            (
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>()
            ));
            SalesFixture.Register(() => Address.Create
            (
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>(),
                SalesFixture.Create<string>()
            ));
            SalesFixture.Register(() => EntityId.Create(SalesFixture.Create<Guid>()));
            SalesFixture.Register(() => ProductQuantity.Create(SalesFixture.CreateInt(1, 10)));
            SalesFixture.Register(() => Price.Create(SalesFixture.CreateDecimal(10, 100)));
        }
    }
}