using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignUpExternalCommand
(
    string UserName, 
    string AssociateEmail, 
    bool AssociateAccount, 
    string ConfirmationUrl
) : ICommand;
    

internal class SignUpExternalCommandValidator : AbstractValidator<SignUpExternalCommand>
{
    public SignUpExternalCommandValidator()
    {
        RuleFor(c => c.AssociateAccount).NotEmpty();
        RuleFor(c => c.ConfirmationUrl).NotEmpty();
    }
}