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
        RuleFor(av => av.Type).NotEmpty();
            
        When(av => av.Type == AccountVerificationType.PhoneNumber, () =>
        {
            RuleFor(av => av.CountryCodeNumber).NotEmpty();
            RuleFor(av => av.PhoneNumber).NotEmpty().PhoneNumber();
        }).Otherwise(() =>
        {
            RuleFor(av => av.ConfirmationUrl).NotEmpty();
        });
    }
}