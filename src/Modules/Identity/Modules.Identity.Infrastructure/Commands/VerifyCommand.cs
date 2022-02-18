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
        RuleFor(av => av.Type).NotEmpty();
       
        When(av => av.Type == AccountVerificationType.PhoneNumber, () =>
        {
            RuleFor(av => av.PhoneNumber).NotEmpty().PhoneNumber();
            RuleFor(av => av.Token).NotEmpty();
        }).Otherwise
        (() => {
            RuleFor(av => av.Token).NotEmpty().Base64Encoded();
        });
    }
}