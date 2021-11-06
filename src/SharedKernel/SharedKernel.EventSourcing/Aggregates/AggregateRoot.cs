﻿using System;
using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Domain.ValueObjects;
using VShop.SharedKernel.Infrastructure.Messaging;
using VShop.SharedKernel.Infrastructure.Messaging.Events;

namespace VShop.SharedKernel.EventSourcing.Aggregates
{
    public abstract class AggregateRoot<TKey>
        where TKey : ValueObject
    {
        private readonly IList<IDomainEvent> _domainEvents = new List<IDomainEvent>();
        private readonly IList<IIntegrationEvent> _integrationEvents = new List<IIntegrationEvent>();

        public TKey Id { get; protected set; }
        public int Version { get; private set; } = -1;
        public Guid CorrelationId { get; set; }
        public Guid MessageId { get; set; }

        protected abstract void When(IDomainEvent @event);

        protected void Apply(IDomainEvent @event)
        {
            When(@event);
            SetCausation(@event);
            SetCorrelation(@event);
            _domainEvents.Add(@event);
        }

        public void Apply(IIntegrationEvent @event)
        {
            SetCausation(@event);
            SetCorrelation(@event);
            _integrationEvents.Add(@event);
        }
        
        public void Load(IEnumerable<IMessage> history)
        {
            foreach (IMessage message in history)
            {
                if(message is IDomainEvent domainEvent) When(domainEvent);
                Version++;
            }
        }

        public IEnumerable<IDomainEvent> GetDomainEvents()
            => _domainEvents.AsEnumerable();
        
        public IEnumerable<IIntegrationEvent> GetIntegrationEvents()
            => _integrationEvents.AsEnumerable();
        
        public IEnumerable<IMessage> GetAllMessages()
            => _domainEvents.Concat(_integrationEvents.Cast<IMessage>()).AsEnumerable();

        public void ClearAllMessages()
        {
            _domainEvents.Clear();
            _integrationEvents.Clear();
        }

        protected void ApplyToEntity(IInternalEventHandler entity, IDomainEvent @event) 
            => entity?.Handle(@event);
        
        private void SetCausation(IMessage @event) 
            => @event.CausationId = MessageId;
        
        private void SetCorrelation(IMessage @event) 
            => @event.CorrelationId = CorrelationId;
    }
}