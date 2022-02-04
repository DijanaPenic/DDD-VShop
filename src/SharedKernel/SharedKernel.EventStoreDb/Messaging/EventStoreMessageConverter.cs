using System;
using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

using Uuid = EventStore.Client.Uuid;

namespace VShop.SharedKernel.EventStoreDb.Messaging;

public class EventStoreMessageConverter : IEventStoreMessageConverter
{
    private readonly IEventStoreSerializer _eventStoreSerializer;
    private readonly IMessageRegistry _messageRegistry;
    private readonly IClockService _clockService;

    public EventStoreMessageConverter
    (
        IEventStoreSerializer eventStoreSerializer,
        IMessageRegistry messageRegistry,
        IClockService clockService
    )
    {
        _eventStoreSerializer = eventStoreSerializer;
        _messageRegistry = messageRegistry;
        _clockService = clockService;
    }

    public MessageEnvelope<TMessage> ToMessage<TMessage>(ResolvedEvent resolvedEvent)
        where TMessage : IMessage
    {
        object data = _eventStoreSerializer.Deserialize
        (
            resolvedEvent.Event.Data.Span.ToArray(),
            _messageRegistry.GetType(resolvedEvent.Event.EventType)
        );

        object UpcastMessage() => _messageRegistry.TryTransform
        (
            resolvedEvent.Event.EventType,
            data,
            out object transformed
        ) ? transformed : data;

        if (UpcastMessage() is not TMessage message) return default;

        MessageMetadata messageMetadata = _eventStoreSerializer.Deserialize
            <MessageMetadata>(resolvedEvent.Event.Metadata.Span.ToArray());

        return new MessageEnvelope<TMessage>
        (
            message,
            messageMetadata.ToMessageContext()
        );
    }

    public EventData FromMessage(MessageEnvelope<IMessage> messageEnvelope)
    {
        (IMessage message, IMessageContext messageContext) = messageEnvelope;
        string typeName = _messageRegistry.GetName(message.GetType());
            
        MessageMetadata messageMetadata = new
        (
            messageContext.MessageId,
            messageContext.Context.RequestId,
            messageContext.Context.CorrelationId,
            messageContext.Context.Identity.Id,
            _clockService.Now
        );

        Guid deterministicId = DeterministicGuid.Create
        (
            messageContext.Context.RequestId,
            $"{typeName}" // TODO - enhance?
        );
            
        return new EventData
        (
            Uuid.FromGuid(deterministicId),
            typeName,
            _eventStoreSerializer.Serialize(message),
            _eventStoreSerializer.Serialize(messageMetadata),
            _eventStoreSerializer.ContentType
        );
    }
}