using FluentValidation;

using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Queries;

internal record IsClientAuthenticatedQuery(Guid ClientId, string ClientSecret) : IQuery<bool>;

internal class IsClientAuthenticatedQueryValidator : AbstractValidator<IsClientAuthenticatedQuery>
{
    public IsClientAuthenticatedQueryValidator()
    {
        RuleFor(q => q.ClientId).NotEmpty();
        RuleFor(q => q.ClientSecret).NotEmpty();
    }
}