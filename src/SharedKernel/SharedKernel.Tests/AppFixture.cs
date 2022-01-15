using System;
using System.Net.Mail;
using AutoFixture;

using VShop.SharedKernel.Tests.Extensions;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.Tests
{
    public static class AppFixture
    {
        public static Fixture CommonFixture { get; }

        static AppFixture()
        {
            CommonFixture = new Fixture();
            
            CommonFixture.Register(() => EmailAddress.Create(CommonFixture.Create<MailAddress>().Address).Data);
            CommonFixture.Register(() => PhoneNumber.Create("+385929551178").Data);
            CommonFixture.Register(() => FullName.Create
            (
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>()
            ).Data);
            CommonFixture.Register(() => Address.Create
            (
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>(),
                CommonFixture.Create<string>()
            ).Data);
            CommonFixture.Register(() => EntityId.Create(CommonFixture.Create<Guid>()).Data);
            CommonFixture.Register(() => ProductQuantity.Create(CommonFixture.CreateInt(1, 10)).Data);
            CommonFixture.Register(() => Price.Create(CommonFixture.CreateDecimal(10, 100)).Data);
            CommonFixture.Register(() => Discount.Create(CommonFixture.CreateInt(0, 100)).Data);
            CommonFixture.Register(Uuid.NewSequentialUuId);
            CommonFixture.Register(() => CommonFixture.CreateDecimal(10, 100).ToMoney());
        } 
    }
}