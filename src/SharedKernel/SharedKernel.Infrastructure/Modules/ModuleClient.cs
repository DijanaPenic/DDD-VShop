using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public class ModuleClient : IModuleClient
{
    private readonly IModuleSerializer _moduleSerializer;
    private readonly IMessageContextProvider _messageContextProvider;

    public ModuleClient
    (
        IModuleSerializer moduleSerializer,
        IMessageContextProvider messageContextProvider
    )
    {
        _moduleSerializer = moduleSerializer;
        _messageContextProvider = messageContextProvider;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        Type messageType = message.GetType();

        IList<ModuleBroadcastRegistration> registrations = ModuleRegistry.GetBroadcastRegistrations(messageType.Name)
            .Where(r => r.ReceiverType != messageType)
            .ToList();

        List<Task> tasks = new();
        foreach (ModuleBroadcastRegistration registration in registrations)
        {
            object receiverMessage = TranslateType(message, registration.ReceiverType);
            
            IMessageContext messageContext = _messageContextProvider.Get((IMessage)message);
            tasks.Add(registration.Action(receiverMessage, messageContext, cancellationToken));
        }

        await Task.WhenAll(tasks);
    }
    
    private object TranslateType(object message, Type type)
        => _moduleSerializer.Deserialize(_moduleSerializer.Serialize(message), type);
}