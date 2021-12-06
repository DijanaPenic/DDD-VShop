using Xunit;
using System.Linq;
using FluentAssertions;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.Modules.Sales.Domain.Events;
using VShop.Modules.Sales.Domain.Models.ShoppingCart;

namespace VShop.Modules.Sales.Testing
{
    public class ShoppingCartUnitTests
    {
        public ShoppingCartUnitTests() { }
        
        [Fact]
        public void Creating_a_shopping_cart()
        {
            // Arrange
            EntityId shoppingCartId = EntityId.Create(SequentialGuid.Create());
            EntityId customerId = EntityId.Create(SequentialGuid.Create());
            const int customerDiscount = 5;
            
            ShoppingCart sut = new();
            
            // Act
            Result result = sut.Create(shoppingCartId, customerId, customerDiscount);
            IDomainEvent[] domainEvents = sut.GetOutgoingDomainEvents().ToArray();
            
            // Assert
            result.Should().Be(Result.Success);
            
            domainEvents.Should().HaveCount(1);
            domainEvents.Should().BeOfType<ShoppingCartCreatedDomainEvent>();
        }
    }
}