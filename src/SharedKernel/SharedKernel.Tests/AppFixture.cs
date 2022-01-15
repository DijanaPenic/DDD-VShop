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
        private static Fixture _commonFixture;

        public static Fixture CommonFixture => _commonFixture ??= GetCommonFixture();

        private static Fixture GetCommonFixture()
        {
            Fixture commonFixture = new();
            
            commonFixture.Register(() => EmailAddress.Create(commonFixture.Create<MailAddress>().Address).Data);
            commonFixture.Register(() => PhoneNumber.Create("+385929551178").Data);
            commonFixture.Register(() => FullName.Create
            (
                commonFixture.Create<string>(),
                commonFixture.Create<string>(),
                commonFixture.Create<string>()
            ).Data);
            commonFixture.Register(() => Address.Create
            (
                commonFixture.Create<string>(),
                commonFixture.Create<string>(),
                commonFixture.Create<string>(),
                commonFixture.Create<string>(),
                commonFixture.Create<string>()
            ).Data);
            commonFixture.Register(() => EntityId.Create(commonFixture.Create<Guid>()).Data);
            commonFixture.Register(() => ProductQuantity.Create(commonFixture.CreateInt(1, 10)).Data);
            commonFixture.Register(() => Price.Create(commonFixture.CreateDecimal(10, 100)).Data);
            commonFixture.Register(() => Discount.Create(commonFixture.CreateInt(0, 100)).Data);
            commonFixture.Register(Uuid.NewSequentialUuId);
            commonFixture.Register(() => commonFixture.CreateDecimal(10, 100).ToMoney());

            return commonFixture;
        }
    }
}