using System;

namespace VShop.SharedKernel.EventSourcing
{
    public interface IAggregateState<T>
    {
        T When(T state, object @event);

        string GetStreamName(Guid id);

        string StreamName { get; }

        long Version { get; }
    }
}