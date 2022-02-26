using FluentValidation;

using VShop.Modules.Identity.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Queries;

public record GetAuthenticatorKeyQuery(Guid UserId) : IQuery<AuthenticatorKey>;

internal class GetAuthenticatorKeyQueryValidator : AbstractValidator<GetAuthenticatorKeyQuery>
{
    public GetAuthenticatorKeyQueryValidator()
    {
        RuleFor(q => q.UserId).NotEmpty();
    }
}