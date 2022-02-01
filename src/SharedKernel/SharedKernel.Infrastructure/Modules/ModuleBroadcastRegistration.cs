using System;
using System.Threading;
using System.Threading.Tasks;

namespace VShop.SharedKernel.Infrastructure.Modules;

public sealed class ModuleBroadcastRegistration
{
    public Type ReceiverType { get; }
    public Func<object, CancellationToken, Task> Action { get; }
    public string Key => ReceiverType.Name;

    public ModuleBroadcastRegistration(Type receiverType, Func<object, CancellationToken, Task> action)
    {
        ReceiverType = receiverType;
        Action = action;
    }
}