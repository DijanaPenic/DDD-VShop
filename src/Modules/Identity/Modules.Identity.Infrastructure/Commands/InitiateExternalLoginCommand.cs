using FluentValidation;
using Microsoft.AspNetCore.Authentication;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record InitiateExternalLoginCommand(string Provider, string ReturnUrl) : ICommand<AuthenticationProperties>;

internal class InitiateExternalLoginCommandValidator : AbstractValidator<InitiateExternalLoginCommand>
{
    public InitiateExternalLoginCommandValidator()
    {
        RuleFor(c => c.Provider).NotEmpty();
        RuleFor(c => c.ReturnUrl).NotEmpty();
    }
}