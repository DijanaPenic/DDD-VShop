using System;
using System.Net.Mail;
using AutoFixture;

using VShop.SharedKernel.Tests.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Tests
{
    public static class AppFixture
    {
        public static Fixture CommonFixture { get; }

        static AppFixture()
        {
            CommonFixture = new Fixture();
            
            CommonFixture.Register(() => EmailAddress.Create(CommonFixture.Create<MailAddress>().Address).Value);
            CommonFixture.Register(() => PhoneNumber.Create("+385929551178").Value);
            CommonFixture.Register(() => FullName.Create
            (
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>()
            ).Value);
            CommonFixture.Register(() => Address.Create
            (
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>()
            ).Value);
            CommonFixture.Register(() => EntityId.Create(CommonFixture.Create<Guid>()).Value);
            CommonFixture.Register(() => ProductQuantity.Create(CommonFixture.CreateInt(1, 10)).Value);
            CommonFixture.Register(() => Price.Create(CommonFixture.CreateDecimal(10, 100)).Value);
            CommonFixture.Register(() => Discount.Create(CommonFixture.CreateInt(0, 100)).Value);
        } 
    }
}