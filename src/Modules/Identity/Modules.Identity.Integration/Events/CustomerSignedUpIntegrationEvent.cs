using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class CustomerSignedUpIntegrationEvent : IIntegrationEvent
{
    public CustomerSignedUpIntegrationEvent(Guid userId)
    {
        UserId = userId;
    }
}