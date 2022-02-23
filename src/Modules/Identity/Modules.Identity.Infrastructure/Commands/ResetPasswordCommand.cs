using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record ResetPasswordCommand(Guid UserId, string Password, string ConfirmPassword, string Token) : ICommand;

internal class ResetPasswordCommandValidator : AbstractValidator<ResetPasswordCommand>
{
    public ResetPasswordCommandValidator()
    {
        RuleFor(c => c.Password).NotEmpty().Password();

        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .Equal(c => c.Password)
            .WithMessage("The new password and confirmation password must match.");

        RuleFor(c => c.Token).NotEmpty().Base64Encoded();
        RuleFor(c => c.UserId).NotEmpty();
    }
}