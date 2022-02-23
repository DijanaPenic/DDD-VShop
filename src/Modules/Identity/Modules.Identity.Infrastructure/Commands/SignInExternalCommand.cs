using FluentValidation;

using VShop.Modules.Identity.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignInExternalCommand(string ConfirmationUrl) : ICommand<SignInInfo>;

internal class SignInExternalCommandValidator : AbstractValidator<SignInExternalCommand>
{
    public SignInExternalCommandValidator()
    {
        RuleFor(c => c.ConfirmationUrl).NotEmpty();
    }
}