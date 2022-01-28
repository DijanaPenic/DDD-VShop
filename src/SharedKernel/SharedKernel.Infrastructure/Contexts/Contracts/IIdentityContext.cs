using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Contexts.Contracts;

public interface IIdentityContext
{
    bool IsAuthenticated { get; }
    public Guid Id { get; }
    string Role { get; }
    IDictionary<string, IEnumerable<string>> Claims { get; }
    bool IsUser();
    bool IsAdmin();
}