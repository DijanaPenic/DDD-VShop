using FluentValidation;

using VShop.SharedKernel.Application.Extensions;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Commands;

internal record SignUpCommand(string UserName, string Email, string Password, string ConfirmPassword) : ICommand;

internal class SignUpRequestValidator : AbstractValidator<SignUpCommand>
{
    public SignUpRequestValidator()
    {
        RuleFor(r => r.UserName).NotEmpty();
        RuleFor(r => r.Email).NotEmpty().EmailAddress();
        RuleFor(r => r.Password).NotEmpty().Password();
        
        RuleFor(r => r.ConfirmPassword)
            .NotEmpty()
            .Equal(r => r.Password)
            .WithMessage("The new password and confirmation password must match.");
    }
}