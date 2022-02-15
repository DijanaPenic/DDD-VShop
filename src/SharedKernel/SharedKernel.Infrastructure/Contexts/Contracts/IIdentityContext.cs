using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IIdentityContext
{
    bool IsAuthenticated { get; }
    public Guid UserId { get; }
    public Guid ClientId { get; }
    public IList<string> Roles { get; }
    IDictionary<string, IEnumerable<string>> Claims { get; }
    bool IsAdmin();
}