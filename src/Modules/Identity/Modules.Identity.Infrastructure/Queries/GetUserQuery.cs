using FluentValidation;

using VShop.Modules.Identity.Infrastructure.DAL.Entities;
using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Queries;

public record GetUserQuery(Guid UserId) : IQuery<User>;

internal class GetUserQueryValidator : AbstractValidator<GetUserQuery>
{
    public GetUserQueryValidator()
    {
        RuleFor(q => q.UserId).NotEmpty();
    }
}