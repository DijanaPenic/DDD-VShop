using FluentValidation;

using VShop.Modules.Identity.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignInByTwoFactorCommand(string Code, bool UseRecoveryCode) : ICommand<SignInInfo>;

internal class SignInByTwoFactorCommandCommandValidator : AbstractValidator<SignInByTwoFactorCommand>
{
    public SignInByTwoFactorCommandCommandValidator()
    {
        RuleFor(c => c.Code).NotEmpty().Length(6);
    }
}