using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Policies;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.EventStoreDb;

public class CustomEventStoreClient
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IEventStoreMessageConverter _eventStoreMessageConverter;
    private readonly IMessageContextRegistry _messageContextRegistry;

    public CustomEventStoreClient
    (
        EventStoreClient eventStoreClient,
        IEventStoreMessageConverter eventStoreMessageConverter,
        IMessageContextRegistry messageContextRegistry
    )
    {
        _eventStoreClient = eventStoreClient;
        _eventStoreMessageConverter = eventStoreMessageConverter;
        _messageContextRegistry = messageContextRegistry;
    }

    public Task AppendToStreamAsync
    (
        string streamSuffix,
        int expectedVersion,
        IEnumerable<IMessage> messages,
        CancellationToken cancellationToken = default
    ) => RetryWrapper.ExecuteAsync((ct) => _eventStoreClient.AppendToStreamAsync
    (
        GetStreamName(streamSuffix),
        StreamRevision.FromInt64(expectedVersion),
        GetEventData(messages),
        cancellationToken: ct
    ), cancellationToken);

    public Task AppendToStreamAsync
    (
        string streamSuffix,
        StreamState expectedState,
        IEnumerable<IMessage> messages,
        CancellationToken cancellationToken = default
    ) => RetryWrapper.ExecuteAsync((ct) => _eventStoreClient.AppendToStreamAsync
    (
        GetStreamName(streamSuffix),
        expectedState,
        GetEventData(messages),
        cancellationToken: ct
    ), cancellationToken);

    public async Task<IReadOnlyList<MessageEnvelope<TMessage>>> ReadStreamForwardAsync<TMessage>
    (
        string streamSuffix,
        CancellationToken cancellationToken = default
    ) where TMessage : IMessage
    {
        EventStoreClient.ReadStreamResult result = await RetryWrapper
            .ExecuteAsync((ct) => _eventStoreClient.ReadStreamAsync
            (
                Direction.Forwards,
                GetStreamName(streamSuffix),
                StreamPosition.Start,
                cancellationToken: ct
            ), cancellationToken);

        if ((await result.ReadState) is ReadState.StreamNotFound) return new List<MessageEnvelope<TMessage>>();

        IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

        return messages.Select(m => _eventStoreMessageConverter.ToMessage<TMessage>(m)).ToList();
    }

    private string GetStreamName(string value) => $"{_eventStoreClient.ConnectionName}/{value}".ToSnakeCase();
    
    private IReadOnlyList<EventData> GetEventData<TMessage>(IEnumerable<TMessage> messages) 
        where TMessage : IMessage
        => messages.Select(message =>
        {
            IMessageContext messageContext = _messageContextRegistry.Get(message);
            return _eventStoreMessageConverter.FromMessage(new MessageEnvelope<IMessage>(message, messageContext));
        }).ToList();
}