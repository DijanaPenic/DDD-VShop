using FluentValidation;

using VShop.Modules.Identity.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record RenewAuthenticatorKeyCommand(Guid UserId) : ICommand<AuthenticatorKey>;

internal class RenewAuthenticatorKeyCommandValidator : AbstractValidator<RenewAuthenticatorKeyCommand>
{
    public RenewAuthenticatorKeyCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
    }
}