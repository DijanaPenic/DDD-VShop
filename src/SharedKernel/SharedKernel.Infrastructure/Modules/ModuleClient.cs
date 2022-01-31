using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public class ModuleClient : IModuleClient
{
    private readonly IModuleSerializer _moduleSerializer;

    public ModuleClient(IModuleSerializer moduleSerializer) => _moduleSerializer = moduleSerializer;

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        Type messageType = message.GetType();

        IList<ModuleBroadcastRegistration> registrations = ModuleRegistry.GetBroadcastRegistrations(messageType.Name)
            .Where(r => r.ReceiverType != messageType)
            .ToList();

        List<Task> tasks = new();
        foreach (ModuleBroadcastRegistration registration in registrations)
        {
            Func<object, CancellationToken, Task> action = registration.Action;
            object receiverMessage = TranslateType(message, registration.ReceiverType);

            tasks.Add(action(receiverMessage, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
    
    private object TranslateType(object message, Type type)
        => _moduleSerializer.Deserialize(_moduleSerializer.Serialize(message), type);
}