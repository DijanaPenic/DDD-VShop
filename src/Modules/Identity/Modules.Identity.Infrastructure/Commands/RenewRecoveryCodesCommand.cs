using FluentValidation;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record RenewRecoveryCodesCommand(Guid UserId, int Number) : ICommand<RecoveryCodes>;

internal record RecoveryCodes(string[] Codes);

internal class RenewRecoveryCodesCommandValidator : AbstractValidator<RenewRecoveryCodesCommand>
{
    public RenewRecoveryCodesCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Number).GreaterThan(0);
    }
}