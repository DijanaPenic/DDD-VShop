using System;
using System.Net.Mail;
using AutoFixture;
using VShop.Modules.Sales.API.Application.Commands.Shared;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;
using VShop.SharedKernel.Domain.Enums;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.Modules.Sales.Tests
{
    public class AppFixture //: IDisposable
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
        }
        
        public ShoppingCart GetShoppingCartForCheckoutFixture()
        {
            ShoppingCart shoppingCart = new();

            shoppingCart.Create
            (
                AutoFixture.Create<EntityId>(),
                AutoFixture.Create<EntityId>(),
                AutoFixture.CreateInt(0, 100)
            );
            
            while(!shoppingCart.HasMinAmountForCheckout)
            {
                ShoppingCartItemCommandDto shoppingCartItem = AutoFixture.Create<ShoppingCartItemCommandDto>();
                shoppingCart.AddProduct
                (
                    EntityId.Create(shoppingCartItem.ProductId),
                    ProductQuantity.Create(shoppingCartItem.Quantity),
                    Price.Create(shoppingCartItem.UnitPrice)
                );
            };
            
            shoppingCart.Customer.SetDeliveryAddress(AutoFixture.Create<Address>());
            shoppingCart.Customer.SetContactInformation
            (
                AutoFixture.Create<FullName>(),
                AutoFixture.Create<EmailAddress>(),
                AutoFixture.Create<PhoneNumber>(),
                AutoFixture.Create<GenderType>()
            );

            return shoppingCart;
        }
        
        // public void Dispose()
        // {
        //     // ... clean up test data ...
        // }
    }
}