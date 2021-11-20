﻿using System;
using System.Threading;
using System.Threading.Tasks;

using VShop.Modules.Sales.Domain.Events;
using VShop.SharedKernel.EventSourcing.Repositories;
using VShop.SharedKernel.EventSourcing.ProcessManagers;
using VShop.SharedKernel.Messaging.Commands;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Application.ProcessManagers
{
    // TODO - should I implement my own event publisher so that I can use customer errors instead of the exception-driven approach?
    internal class OrderingProcessManagerHandler :
        ProcessManagerHandler<OrderingProcessManager>,
        IDomainEventHandler<ShoppingCartCheckoutRequestedDomainEvent>,
        IDomainEventHandler<OrderPlacedDomainEvent>
        //ICommandHandler<ReminderCommand>
    {
        public OrderingProcessManagerHandler(IProcessManagerRepository<OrderingProcessManager> processManagerRepository)
            : base(processManagerRepository)
        {
            
        }
        
        public Task Handle(ShoppingCartCheckoutRequestedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);

        public Task Handle(OrderPlacedDomainEvent @event, CancellationToken cancellationToken)
            => TransitionAsync(@event.OrderId, @event, cancellationToken);

        // public Task<Result> Handle(ReminderCommand command, CancellationToken cancellationToken)
        //     => ExecuteAsync(command.ProcessId, command, cancellationToken);
    }
    
    // TODO - move to some other class
    // public record ReminderCommand : Command
    // {
    //     public string Type { get; init; }
    //     public Guid ProcessId { get; init; }
    //     public int Status { get; init; }
    //     public string Command { get; init; }
    // }
}