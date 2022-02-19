using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignUpCommand
(
    string UserName,
    string Email,
    string Password,
    string ConfirmPassword,
    string ActivationUrl
) : ICommand<Guid>;

internal class SignUpCommandValidator : AbstractValidator<SignUpCommand>
{
    public SignUpCommandValidator()
    {
        RuleFor(c => c.UserName).NotEmpty();
        RuleFor(c => c.Email).NotEmpty().EmailAddress();
        RuleFor(c => c.ActivationUrl).NotEmpty();
        
        RuleFor(c => c.Password).NotEmpty().Password();
        RuleFor(c => c.ConfirmPassword)
            .NotEmpty()
            .Equal(c => c.Password)
            .WithMessage("Password and confirmation password must match.");
    }
}