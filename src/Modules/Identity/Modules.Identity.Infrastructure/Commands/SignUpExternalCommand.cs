using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignUpExternalCommand
(
    string UserName, 
    string AssociateEmail, 
    bool AssociateToAccount, 
    string ConfirmationUrl
) : ICommand;

internal class SignUpExternalCommandValidator : AbstractValidator<SignUpExternalCommand>
{
    public SignUpExternalCommandValidator()
    {
        When(c => c.AssociateToAccount, () =>
        {
            RuleFor(c => c.ConfirmationUrl).NotEmpty();
            RuleFor(c => c.AssociateEmail).NotEmpty();
        }).Otherwise(() =>
        {
            RuleFor(c => c.UserName).NotEmpty();
        });
    }
}