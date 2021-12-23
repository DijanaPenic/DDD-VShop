using System;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Serilog;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Messaging;
using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure.Helpers;
using VShop.SharedKernel.EventStoreDb.Extensions;
using VShop.SharedKernel.EventStoreDb.Subscriptions;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure;
using VShop.SharedKernel.EventStoreDb.Subscriptions.Infrastructure.Entities;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventBus _eventBus;

        public IntegrationEventPublisher
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IEventBus eventBus
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventBus = eventBus;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IMessage message = resolvedEvent.DeserializeData<IMessage>();
            if (message is not IIntegrationEvent integrationEvent) return;
            
            _logger.Debug("Projecting integration event: {Message}", integrationEvent);
            
            using IServiceScope scope = _serviceProvider.CreateScope();
            SubscriptionContext subscriptionContext = scope.ServiceProvider.GetRequiredService<SubscriptionContext>();

            IExecutionStrategy strategy = subscriptionContext.Database.CreateExecutionStrategy();
            await strategy.ExecuteAsync(async () =>
            {
                await subscriptionContext.BeginTransactionAsync(cancellationToken);

                try
                {
                    await _eventBus.Publish(integrationEvent, EventPublishStrategy.SyncStopOnException, cancellationToken);
                }
                catch (Exception ex)
                {
                    subscriptionContext.MessageDeadLetterLogs.Add(new MessageDeadLetterLog
                    {
                        Id = SequentialGuid.Create(),
                        StreamId = resolvedEvent.OriginalStreamId,
                        MessageType = resolvedEvent.Event.EventType,
                        MessageId = resolvedEvent.Event.EventId.ToGuid(),
                        MessageData = Encoding.UTF8.GetString(resolvedEvent.Event.Data.Span),
                        Status = MessageProcessingStatus.Failed,
                        Error = $"{ex.Message}{ex.StackTrace}"
                    });
                    await subscriptionContext.SaveChangesAsync(cancellationToken);
                }
                
                await checkpointUpdate(subscriptionContext);
                
                await subscriptionContext.CommitCurrentTransactionAsync(cancellationToken);
            });
        }
    }
}