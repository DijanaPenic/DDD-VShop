using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record VerifyExternalCommand(Guid UserId, string Token) : ICommand;

internal class VerifyExternalCommandValidator : AbstractValidator<VerifyExternalCommand>
{
    public VerifyExternalCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Token).NotEmpty();
    }
}