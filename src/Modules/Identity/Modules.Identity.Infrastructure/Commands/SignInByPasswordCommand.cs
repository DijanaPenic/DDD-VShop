using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Models;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignInByPasswordCommand(string UserName, string Password) : ICommand<SignInInfo>;

internal class SignInByPasswordCommandValidator : AbstractValidator<SignInByPasswordCommand>
{
    public SignInByPasswordCommandValidator()
    {
        RuleFor(c => c.UserName).NotEmpty();
        RuleFor(c => c.Password).NotEmpty().Password();
    }
}