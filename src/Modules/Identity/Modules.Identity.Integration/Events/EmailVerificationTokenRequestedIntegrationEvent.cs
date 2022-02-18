using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class EmailVerificationTokenRequestedIntegrationEvent : IIntegrationEvent
{
    public EmailVerificationTokenRequestedIntegrationEvent
    (
        Guid userId,
        string token,
        string email,
        string confirmationUrl
    )
    {
        UserId = userId;
        Token = token;
        Email = email;
        ConfirmationUrl = confirmationUrl;
    }
}