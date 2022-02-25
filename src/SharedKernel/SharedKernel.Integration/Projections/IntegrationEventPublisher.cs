using Serilog;
using Newtonsoft.Json;
using EventStore.Client;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;

using VShop.SharedKernel.Subscriptions;
using VShop.SharedKernel.Subscriptions.DAL;
using VShop.SharedKernel.Subscriptions.DAL.Entities;
using VShop.SharedKernel.EventStoreDb.Messaging.Contracts;
using VShop.SharedKernel.Infrastructure.Types;
using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

namespace VShop.SharedKernel.Integration.Projections
{
    public class IntegrationEventPublisher : ISubscriptionHandler
    {
        private readonly ILogger _logger;
        private readonly IServiceProvider _serviceProvider;
        private readonly IEventStoreMessageConverter _eventStoreMessageConverter;
        private readonly IEventDispatcher _eventDispatcher;

        public IntegrationEventPublisher
        (
            ILogger logger,
            IServiceProvider serviceProvider,
            IEventStoreMessageConverter eventStoreMessageConverter,
            IEventDispatcher eventDispatcher
        )
        {
            _logger = logger;
            _serviceProvider = serviceProvider;
            _eventStoreMessageConverter = eventStoreMessageConverter;
            _eventDispatcher = eventDispatcher;
        }

        public async Task ProjectAsync
        (
            ResolvedEvent resolvedEvent,
            Func<SubscriptionDbContext, Task> checkpointUpdate,
            CancellationToken cancellationToken = default
        )
        {
            IIntegrationEvent integrationEvent = _eventStoreMessageConverter
                .ToMessage<IIntegrationEvent>(resolvedEvent)?.Message;
                
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
                    await _eventDispatcher.PublishAsync(integrationEvent, cancellationToken);
                }
                catch (Exception ex)
                {
                    subscriptionDbContext.MessageDeadLetterLogs.Add(new MessageDeadLetterLog
                    {
                        Id = SequentialGuid.Create(),
                        StreamId = resolvedEvent.OriginalStreamId,
                        MessageType = resolvedEvent.Event.EventType,
                        MessageId = resolvedEvent.Event.EventId.ToGuid(),
                        MessageData = JsonConvert.SerializeObject(integrationEvent, DefaultJsonSerializer.Settings),
                        Status = MessageProcessingStatus.Failed,
                        Error = $"{ex.Message}{ex.StackTrace}"
                    });
                    await subscriptionDbContext.SaveChangesAsync(cancellationToken);
                }
                
                await checkpointUpdate(subscriptionDbContext);
                await subscriptionDbContext.CommitTransactionAsync(cancellationToken);
            });
        }
    }
}