using AutoFixture;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Tests.Customizations
{
    public class ShoppingCartCustomization : ICustomization
    {
        public void Customize(IFixture fixture)
        {
            fixture.Customize<ShoppingCart>(composer =>
                composer.FromFactory(() =>
                {
                    ShoppingCart shoppingCart = new();

                    shoppingCart.Create
                    (
                        fixture.Create<EntityId>(),
                        fixture.Create<EntityId>(),
                        fixture.Create<Discount>()
                    );
            
                    while(!shoppingCart.HasMinAmountForCheckout)
                    {
                        shoppingCart.AddProduct
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
                        fixture.Create<GenderType>()
                    );

                    return shoppingCart;
                }));
        }
    }
}