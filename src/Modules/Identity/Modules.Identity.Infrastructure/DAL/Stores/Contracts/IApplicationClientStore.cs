using VShop.Modules.Identity.Infrastructure.DAL.Entities;

namespace VShop.Modules.Identity.Infrastructure.DAL.Stores.Contracts;

internal interface IApplicationClientStore
{
    Task<Client> FindClientByKeyAsync(Guid clientId, CancellationToken cancellationToken);
    Task<Client> FindClientByNameAsync(string name, CancellationToken cancellationToken);
}