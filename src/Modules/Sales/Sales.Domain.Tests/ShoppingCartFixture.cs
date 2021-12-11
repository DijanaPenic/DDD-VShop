using AutoFixture;

using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Domain.Tests
{
    // TODO - should this be moved to the Domain project?
    internal static class ShoppingCartFixture
    {
        public static ShoppingCart GetShoppingCartForCheckoutFixture(Fixture autoFixture)
        {
            ShoppingCart shoppingCart = new();

            shoppingCart.Create
            (
                autoFixture.Create<EntityId>(),
                autoFixture.Create<EntityId>(),
                autoFixture.Create<Discount>()
            );
            
            while(!shoppingCart.HasMinAmountForCheckout)
            {
                shoppingCart.AddProduct
                (
                    autoFixture.Create<EntityId>(),
                    autoFixture.Create<ProductQuantity>(),
                    autoFixture.Create<Price>()
                );
            };
            
            shoppingCart.Customer.SetDeliveryAddress(autoFixture.Create<Address>());
            shoppingCart.Customer.SetContactInformation
            (
                autoFixture.Create<FullName>(),
                autoFixture.Create<EmailAddress>(),
                autoFixture.Create<PhoneNumber>(),
                autoFixture.Create<GenderType>()
            );

            return shoppingCart;
        }
    }
}