using Serilog;
using System.Text;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Dispatchers;
using VShop.SharedKernel.Infrastructure.Events.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL;
using VShop.SharedKernel.EventStoreDb.Subscriptions.DAL.Entities;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IMessageRegistry _messageRegistry;
        private readonly IEventDispatcher _eventDispatcher;

        public IntegrationEventPublisher
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IMessageRegistry messageRegistry,
            IEventDispatcher eventDispatcher
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _messageRegistry = messageRegistry;
            _eventDispatcher = eventDispatcher;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IIntegrationEvent integrationEvent = resolvedEvent.Deserialize<IIntegrationEvent>(_messageRegistry);
            if (integrationEvent is null) return;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionDbContext subscriptionDbContext = scope.ServiceProvider.GetRequiredService<SubscriptionDbContext>();

            IExecutionStrategy strategy = subscriptionDbContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await subscriptionDbContext.BeginTransactionAsync(cancellationToken);

                try
                {
                    await _eventDispatcher.PublishAsync
                    (
                        integrationEvent,
                        NotificationDispatchStrategy.SyncStopOnException,
                        cancellationToken
                    );
                }
                catch (Exception ex)
                {
                    subscriptionDbContext.MessageDeadLetterLogs.Add(new MessageDeadLetterLog
                    {
                        Id = SequentialGuid.Create(),
                        StreamId = resolvedEvent.OriginalStreamId,
                        MessageType = resolvedEvent.Event.EventType,
                        MessageId = resolvedEvent.Event.EventId.ToGuid(),
                        
                        // TODO - need to accommodate proto type.
                        MessageData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
                        
                        Status = MessageProcessingStatus.Failed,
                        Error = $"{ex.Message}{ex.StackTrace}"
                    });
                    await subscriptionDbContext.SaveChangesAsync(cancellationToken);
                }
                
                await checkpointUpdate(subscriptionDbContext);
                
                await subscriptionDbContext.CommitCurrentTransactionAsync(cancellationToken);
            });
        }
    }
}