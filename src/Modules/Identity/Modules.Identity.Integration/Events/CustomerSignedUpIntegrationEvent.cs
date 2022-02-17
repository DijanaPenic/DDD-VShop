using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class CustomerSignedUpIntegrationEvent : IIntegrationEvent
{
    public CustomerSignedUpIntegrationEvent(Uuid userId, string emailConfirmationToken)
    {
        UserId = userId;
        EmailConfirmationToken = emailConfirmationToken;
    }
}