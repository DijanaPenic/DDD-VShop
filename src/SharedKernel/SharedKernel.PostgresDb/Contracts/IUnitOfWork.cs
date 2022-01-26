using System;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore.Storage;

namespace VShop.SharedKernel.PostgresDb.Contracts;

public interface IUnitOfWork
{
    IDbContextTransaction CurrentTransaction { get; }
    Task<Guid> ExecuteAsync(Func<Task> action, CancellationToken cancellationToken = default);
    Task<(Guid, TResponse)> ExecuteAsync<TResponse>(Func<Task<TResponse>> action, CancellationToken cancellationToken = default);
}