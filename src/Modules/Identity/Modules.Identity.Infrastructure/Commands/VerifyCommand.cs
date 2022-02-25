using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.Modules.Identity.Infrastructure.Commands.Shared;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record VerifyCommand(Guid UserId, AccountVerificationType Type, string Token, string PhoneNumber) : ICommand;

internal class VerifyCommandValidator : AbstractValidator<VerifyCommand>
{
    public VerifyCommandValidator()
    {
        RuleFor(c => c.Type).NotEmpty();            
        RuleFor(c => c.Token).NotEmpty();
        
        When(c => c.Type is AccountVerificationType.PhoneNumber, () =>
        {
            RuleFor(c => c.PhoneNumber).NotEmpty().PhoneNumber();
        });
    }
}