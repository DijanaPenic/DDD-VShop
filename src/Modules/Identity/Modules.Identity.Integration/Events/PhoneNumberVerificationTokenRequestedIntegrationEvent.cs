using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.Modules.Identity.Integration.Events;

public partial class PhoneNumberVerificationTokenRequestedIntegrationEvent : IIntegrationEvent
{
    public PhoneNumberVerificationTokenRequestedIntegrationEvent
    (
        Guid userId,
        string token,
        string phoneNumber,
        bool isVoiceCall
    )
    {
        UserId = userId;
        Token = token;
        PhoneNumber = phoneNumber;
        IsVoiceCall = isVoiceCall;
    }
}