using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record DisconnectExternalLoginCommand(Guid UserId, string LoginProvider, string ProviderKey) : ICommand;
    

internal class DisconnectExternalLoginCommandValidator : AbstractValidator<DisconnectExternalLoginCommand>
{
    public DisconnectExternalLoginCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.LoginProvider).NotEmpty();
        RuleFor(c => c.ProviderKey).NotEmpty();
    }
}