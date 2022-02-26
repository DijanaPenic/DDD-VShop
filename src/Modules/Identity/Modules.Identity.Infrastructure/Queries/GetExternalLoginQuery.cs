using FluentValidation;

using VShop.Modules.Identity.Infrastructure.Models;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Queries;

public record GetExternalLoginQuery(Guid UserId) : IQuery<List<ExternalLoginInfo>>;

internal class GetExternalLoginQueryValidator : AbstractValidator<GetExternalLoginQuery>
{
    public GetExternalLoginQueryValidator()
    {
        RuleFor(q => q.UserId).NotEmpty();
    }
}