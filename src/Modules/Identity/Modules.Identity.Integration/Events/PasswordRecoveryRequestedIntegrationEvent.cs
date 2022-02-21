using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class PasswordRecoveryRequestedIntegrationEvent : IIntegrationEvent
{
    public PasswordRecoveryRequestedIntegrationEvent(Uuid userId, string token, string email, string userName, string confirmationUrl)
    {
        UserId = userId;
        Token = token;
        Email = email;
        UserName = userName;
        ConfirmationUrl = confirmationUrl;
    }
}