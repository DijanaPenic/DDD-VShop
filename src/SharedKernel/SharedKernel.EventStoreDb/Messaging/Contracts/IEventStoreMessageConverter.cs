using EventStore.Client;

using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Messaging.Contracts;

public interface IEventStoreMessageConverter
{
    MessageEnvelope<TMessage> ToMessage<TMessage>(ResolvedEvent resolvedEvent)
        where TMessage : IMessage;

    EventData FromMessage(MessageEnvelope<IMessage> messageEnvelope);
}