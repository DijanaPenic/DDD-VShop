using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Services;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignInCommand(string UserName, string Password) : ICommand<SignInResponse>;

internal class SignInRequestValidator : AbstractValidator<SignInCommand>
{
    public SignInRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty();
        RuleFor(r => r.Password).NotEmpty().Password();
    }
}