using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class PasswordRecoveryRequestedIntegrationEvent : IIntegrationEvent
{
    public PasswordRecoveryRequestedIntegrationEvent
    (
        Guid userId,
        string email,
        string userName,
        string token,
        string confirmationUrl
    )
    {
        UserId = userId;
        Token = token;
        Email = email;
        UserName = userName;
        ConfirmationUrl = confirmationUrl;
    }
}