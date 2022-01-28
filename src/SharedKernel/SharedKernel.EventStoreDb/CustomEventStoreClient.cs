using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Collections.Generic;
using EventStore.Client;

using VShop.SharedKernel.EventStoreDb.Policies;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.EventStoreDb;

public class CustomEventStoreClient
{
    private readonly EventStoreClient _eventStoreClient;
    private readonly IMessageRegistry _messageRegistry;
    private readonly IMessageContextProvider _messageContextProvider;

    public CustomEventStoreClient
    (
        EventStoreClient eventStoreClient,
        IMessageRegistry messageRegistry,
        IMessageContextProvider messageContextProvider
    )
    {
        _eventStoreClient = eventStoreClient;
        _messageRegistry = messageRegistry;
        _messageContextProvider = messageContextProvider;
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
        messages.ToEventData(_messageRegistry, _messageContextProvider),
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
        messages.ToEventData(_messageRegistry, _messageContextProvider),
        cancellationToken: ct
    ), cancellationToken);

    public async Task<IReadOnlyList<TMessage>> ReadStreamForwardAsync<TMessage>
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

        if ((await result.ReadState) is ReadState.StreamNotFound) return new List<TMessage>();

        IList<ResolvedEvent> messages = await result.ToListAsync(cancellationToken);

        return messages.Select(@event => @event.Deserialize<TMessage>(_messageRegistry)).ToList();
    }

    private string GetStreamName(string value)
        => $"{_eventStoreClient.ConnectionName}/{value}".ToSnakeCase();
}