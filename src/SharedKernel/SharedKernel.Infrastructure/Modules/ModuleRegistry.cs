using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Reflection;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public sealed class ModuleRegistry : IModuleRegistry
{
    private static readonly List<ModuleBroadcastRegistration> BroadcastRegistrations = new();

    public IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string key)
        => BroadcastRegistrations.Where(r => r.Key == key);

    public static void AddBroadcastActions(IServiceProvider serviceProvider, IEnumerable<Assembly> assemblies)
    {
        Type[] commandTypes = assemblies.SelectMany(a => a.GetTypes())
            .Where(t => t.IsClass && typeof(IBaseCommand).IsAssignableFrom(t))
            .ToArray();

        IDispatcher dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        foreach (Type type in commandTypes)
        {
            AddBroadcastAction(type, (command, cancellationToken) =>
                dispatcher.ExecuteCommandAsync(command, cancellationToken));
        }
    }
    
    private static void AddBroadcastAction(Type requestType, Func<object, CancellationToken, Task> action)
    {
        if (string.IsNullOrWhiteSpace(requestType.Namespace))
            throw new InvalidOperationException("Missing namespace.");

        ModuleBroadcastRegistration registration = new(requestType, action);
        BroadcastRegistrations.Add(registration);
    }
}