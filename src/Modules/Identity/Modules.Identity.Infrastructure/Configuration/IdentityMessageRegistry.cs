using VShop.Modules.Identity.Integration.Events;
using VShop.Modules.Identity.Infrastructure.Queries;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Configuration
{
    internal static class IdentityMessageRegistry
    {
        public static IMessageRegistry Initialize()
        {
            MessageRegistry registry = new();

            registry.RegisterMessages();
            registry.RegisterTransformations();

            return registry;
        }
        
        private static void RegisterMessages(this MessageRegistry registry)
        {
            // Configure queries
            registry.Add<IsClientAuthenticatedQuery>(nameof(IsClientAuthenticatedQuery));
            
            // Configure domain events


            // Configure integration events - local

            
            // Configure integration events - remote
        }
        
        private static void RegisterTransformations(this MessageRegistry registry)
        {
            // CAUTION: this is just a showcase example.
            //registry.Register<ShoppingCartCreatedDomainEvent, ShoppingCartCreatedDomainEvent2>(ShoppingCartCreatedDomainEvent2.ConvertFrom);
        }
    }
}