using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Models;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SetPasswordCommand(Guid UserId, string Password) : ICommand;

internal class SetPasswordCommandValidator : AbstractValidator<SetPasswordCommand>
{
    public SetPasswordCommandValidator()
    {
        RuleFor(c => c.UserId).NotEmpty();
        RuleFor(c => c.Password).NotEmpty().Password();
    }
}