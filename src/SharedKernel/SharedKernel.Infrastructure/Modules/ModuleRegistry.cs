using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public static class ModuleRegistry
{
    private static readonly List<ModuleBroadcastRegistration> BroadcastRegistrations = new();

    public static IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistrations(string key)
        => BroadcastRegistrations.Where(r => r.Key == key);

    public static void AddBroadcastActions(IServiceProvider serviceProvider, IEnumerable<Assembly> assemblies)
    {
        Type[] types = assemblies.SelectMany(a => a.GetTypes()).ToArray();
        Type[] commandTypes = types
            .Where(t => t.IsClass && typeof(IBaseCommand).IsAssignableFrom(t))
            .ToArray();
        
        Type[] eventTypes = types
            .Where(t => t.IsClass && typeof(IBaseEvent).IsAssignableFrom(t))
            .ToArray();

        IDispatcher dispatcher = serviceProvider.GetRequiredService<IDispatcher>();
        foreach (Type type in commandTypes)
        {
            AddBroadcastAction(type, (command, cancellationToken) =>
                (Task)dispatcher.GetType().GetMethod(nameof(dispatcher.SendAsync))
                    ?.MakeGenericMethod(type)
                    .Invoke(dispatcher, new[] { command, cancellationToken }));
        }
        
        foreach (Type type in eventTypes)
        {
            AddBroadcastAction(type, (@event, cancellationToken) =>
                (Task)dispatcher.GetType().GetMethod(nameof(dispatcher.PublishAsync))
                    ?.MakeGenericMethod(type)
                    .Invoke(dispatcher, new[] { @event, cancellationToken }));
        }
    }
    
    private static void AddBroadcastAction(Type actionType, Func<object, CancellationToken, Task> action)
    {
        if (string.IsNullOrWhiteSpace(actionType.Namespace))
            throw new InvalidOperationException("Missing namespace.");

        ModuleBroadcastRegistration registration = new(actionType, action);
        BroadcastRegistrations.Add(registration);
    }
}