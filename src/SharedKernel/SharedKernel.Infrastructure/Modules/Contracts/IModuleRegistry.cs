using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModuleRegistry
{
    IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string key);
}