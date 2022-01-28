using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Sales.Domain.Events
{
    // CAUTION: this is just a showcase example. Should not be used!
    public partial class ShoppingCartCreatedDomainEvent2 : IDomainEvent
    {
        public ShoppingCartCreatedDomainEvent2(string description) => Description = description;
        
        public static ShoppingCartCreatedDomainEvent2 ConvertFrom(ShoppingCartCreatedDomainEvent _)
            => new("showcase example");
    }
}