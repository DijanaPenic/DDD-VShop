using VShop.SharedKernel.Infrastructure.Queries.Contracts;

namespace VShop.Modules.Identity.Infrastructure.Queries;

internal record IsClientAuthenticatedQuery(Guid ClientId, string ClientSecret) : IQuery<bool>;