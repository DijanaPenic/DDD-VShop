using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class AccountConfirmationRequestedIntegrationEvent : IIntegrationEvent
{
    public AccountConfirmationRequestedIntegrationEvent
    (
        Guid userId,
        string email,
        string token,
        string confirmationUrl
    )
    {
        UserId = userId;
        Token = token;
        Email = email;
        ConfirmationUrl = confirmationUrl;
    }
}