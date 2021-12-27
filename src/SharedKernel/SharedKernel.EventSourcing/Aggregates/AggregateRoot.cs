﻿using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Messaging.Events;
using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Messaging;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot
    {
        private readonly List<IBaseEvent> _outbox = new();

        public EntityId Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CausationId { get; }
        public Guid CorrelationId { get; }

        protected AggregateRoot(Guid causationId, Guid correlationId)
        {
            CausationId = causationId;
            CorrelationId = correlationId;
        }
        
        protected abstract void ApplyEvent(IDomainEvent @event);

        protected void RaiseEvent(IDomainEvent @event)
        {
            ApplyEvent(@event);
            SetEventIdentification(@event);
            _outbox.Add(@event);
        }
        
        public void RaiseEvent(IIntegrationEvent @event)
        {
            SetEventIdentification(@event);
            _outbox.Add(@event);
        }
        
        public void Load(IEnumerable<IBaseEvent> history)
        {
            (IEnumerable<IBaseEvent> pendingEvents, IEnumerable<IBaseEvent> processedEvents) = 
                history.Split(e => e.CausationId == CausationId);
            
            foreach (IBaseEvent @event in processedEvents)
            {
                if(@event is IDomainEvent domainEvent) ApplyEvent(domainEvent);
                Version++;
            }

            foreach (IBaseEvent @event in pendingEvents)
            {
                switch (@event)
                {
                    case IDomainEvent domainEvent:
                        RaiseEvent(domainEvent);
                        break;
                    case IIntegrationEvent integrationEvent:
                        RaiseEvent(integrationEvent);
                        break;
                }
            }
        }

        public IReadOnlyList<TType> GetOutboxMessages<TType>() => _outbox.OfType<TType>().ToList();

        public IReadOnlyList<IBaseEvent> GetOutboxMessages() => _outbox;

        public int OutboxMessageCount => _outbox.Count;

        public void Clear()
        {
            Version += _outbox.Count;
            _outbox.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) => entity.Handle(@event);
        
        private void SetEventIdentification(IBaseEvent @event)
        {
            @event.CausationId = CausationId;
            @event.CorrelationId = CorrelationId;
        }
    }
}