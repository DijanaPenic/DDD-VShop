using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignInExternalCommand(string ConfirmationUrl) : ICommand;
    

internal class SignInExternalCommandValidator : AbstractValidator<SignInExternalCommand>
{
    public SignInExternalCommandValidator()
    {
        RuleFor(c => c.ConfirmationUrl).NotEmpty();
    }
}