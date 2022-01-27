using AutoFixture;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Tests.Customizations
{
    internal class ShoppingCartCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ShoppingCart>(composer => composer.FromFactory(() =>
            {
                ShoppingCart shoppingCart = ShoppingCart.Create
                (
                    fixture.Create<EntityId>(),
                    fixture.Create<EntityId>(),
                    fixture.Create<Discount>()
                ).Data;
        
                while(!shoppingCart.HasMinAmountForCheckout)
                {
                    shoppingCart.AddProductQuantity
                    (
                        fixture.Create<EntityId>(),
                        fixture.Create<ProductQuantity>(),
                        fixture.Create<Price>()
                    );
                };
        
                shoppingCart.Customer.SetDeliveryAddress(fixture.Create<Address>());
                shoppingCart.Customer.SetContactInformation
                (
                    fixture.Create<FullName>(),
                    fixture.Create<EmailAddress>(),
                    fixture.Create<PhoneNumber>(),
                    fixture.Create<Gender>()
                );

                return shoppingCart;
            }));
        }
    }
}