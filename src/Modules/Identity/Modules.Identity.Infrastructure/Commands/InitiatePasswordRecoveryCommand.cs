using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record InitiatePasswordRecoveryCommand(string Email, string ConfirmationUrl) : ICommand;

internal class InitiatePasswordRecoveryCommandValidator : AbstractValidator<InitiatePasswordRecoveryCommand>
{
    public InitiatePasswordRecoveryCommandValidator()
    {
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.ConfirmationUrl).NotEmpty();
    }
}