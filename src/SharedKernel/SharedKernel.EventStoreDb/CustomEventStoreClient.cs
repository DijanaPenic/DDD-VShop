using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;
using Google.Protobuf;

using VShop.SharedKernel.EventStoreDb.Policies;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Services.Contracts;
using VShop.SharedKernel.Infrastructure.Types;

using Uuid = EventStore.Client.Uuid;
using IMessage = VShop.SharedKernel.Infrastructure.Messaging.Contracts.IMessage;

namespace VShop.SharedKernel.EventStoreDb;

public class CustomEventStoreClient
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IMessageRegistry _messageRegistry;
    private readonly IMessageContextProvider _messageContextProvider;
    private readonly IClockService _clockService;

    public CustomEventStoreClient
    (
        EventStoreClient eventStoreClient,
        IMessageRegistry messageRegistry,
        IMessageContextProvider messageContextProvider,
        IClockService clockService
    )
    {
        _eventStoreClient = eventStoreClient;
        _messageRegistry = messageRegistry;
        _messageContextProvider = messageContextProvider;
        _clockService = clockService;
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

        return messages.Select(message => message.Deserialize<TMessage>(_messageRegistry)).ToList();
    }

    private string GetStreamName(string value)
        => $"{_eventStoreClient.ConnectionName}/{value}".ToSnakeCase();
    
    private IReadOnlyList<EventData> GetEventData<TMessage>(IEnumerable<TMessage> messages) 
        where TMessage : IMessage
        => messages.Select((message, index) =>
        {
            string typeName = _messageRegistry.GetName(message.GetType());
            
            IMessageContext messageContext = _messageContextProvider.Get(message);
            MessageMetadata messageMetadata = new
            (
                messageContext.MessageId,
                messageContext.Context.RequestId,
                messageContext.Context.CorrelationId,
                _clockService.Now
            );

            Guid deterministicId = DeterministicGuid.Create
            (
                messageContext.Context.RequestId,
                $"{typeName}-{index}"
            );
            
            return new EventData
            (
                Uuid.FromGuid(deterministicId),
                typeName,
                message.ToByteArray(),
                messageMetadata.ToByteArray(),
                "application/octet-stream"
            );
        }).ToList();
}