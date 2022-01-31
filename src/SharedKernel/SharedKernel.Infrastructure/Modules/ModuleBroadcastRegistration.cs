using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public sealed class ModuleBroadcastRegistration
{
    public Type ReceiverType { get; }
    public Func<object, IMessageContext, CancellationToken, Task> Action { get; }
    public string Key => ReceiverType.Name;

    public ModuleBroadcastRegistration(Type receiverType, Func<object, IMessageContext, CancellationToken, Task> action)
    {
        ReceiverType = receiverType;
        Action = action;
    }
}