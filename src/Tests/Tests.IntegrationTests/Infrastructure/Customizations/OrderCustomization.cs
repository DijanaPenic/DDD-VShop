using AutoFixture;

using VShop.Modules.Sales.Domain.Models.Ordering;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.Tests.IntegrationTests.Infrastructure.Customizations
{
    internal class OrderCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<Order>(composer => composer.FromFactory(() =>
            {
                Order order = Order.Create
                (
                    fixture.Create<EntityId>(),
                    fixture.Create<Price>(),
                    fixture.Create<EntityId>(),
                    fixture.Create<Discount>(),
                    fixture.Create<FullName>(),
                    fixture.Create<EmailAddress>(),
                    fixture.Create<PhoneNumber>(),
                    fixture.Create<Address>()
                ).Data;

                order.AddOrderLine
                (
                    fixture.Create<EntityId>(),
                    fixture.Create<ProductQuantity>(),
                    fixture.Create<Price>()
                );

                return order;
            }));
        }
    }
}