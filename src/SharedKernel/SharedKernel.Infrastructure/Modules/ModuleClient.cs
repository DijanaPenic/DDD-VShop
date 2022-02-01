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
    private readonly IMessageContextRegistry _messageContextRegistry;

    public ModuleClient
    (
        IModuleSerializer moduleSerializer,
        IMessageContextRegistry messageContextRegistry
    )
    {
        _moduleSerializer = moduleSerializer;
        _messageContextRegistry = messageContextRegistry;
    }

    public async Task PublishAsync(object message, CancellationToken cancellationToken = default)
    {
        Type messageType = message.GetType();

        IList<ModuleBroadcastRegistration> registrations = ModuleRegistry.GetBroadcastRegistrations(messageType.Name)
            .Where(r => r.ReceiverType != messageType)
            .ToList();

        IMessageContext messageContext = _messageContextRegistry.Get((IMessage)message);
        IList<Task> tasks = new List<Task>();
        
        foreach (ModuleBroadcastRegistration registration in registrations)
        {
            object receiverMessage = TranslateType(message, registration.ReceiverType);
            _messageContextRegistry.Set((IMessage)receiverMessage, messageContext);
            
            tasks.Add(registration.Action(receiverMessage, cancellationToken));
        }
        
        await Task.WhenAll(tasks);
    }
    
    private object TranslateType(object message, Type type)
        => _moduleSerializer.Deserialize(_moduleSerializer.Serialize(message), type);
}