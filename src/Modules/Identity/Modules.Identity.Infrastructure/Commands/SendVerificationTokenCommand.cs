using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands.Shared;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SendVerificationTokenCommand : ICommand
{
    public Guid UserId { get; init; }
    public AccountVerificationType Type { get; init; }
    public string ConfirmationUrl { get; init; }
    public string CountryCodeNumber { get; init; }
    public string PhoneNumber { get; init; }
    public bool IsVoiceCall { get; init; }
}

internal class SendVerificationTokenCommandValidator : AbstractValidator<SendVerificationTokenCommand>
{
    public SendVerificationTokenCommandValidator()
    { 
        RuleFor(c => c.Type).NotEmpty();
            
        When(c => c.Type is AccountVerificationType.PhoneNumber, () =>
        {
            RuleFor(c => c.CountryCodeNumber).NotEmpty();
            RuleFor(c => c.PhoneNumber).NotEmpty().PhoneNumber();
        }).Otherwise(() =>
        {
            RuleFor(c => c.ConfirmationUrl).NotEmpty();
        });
    }
}