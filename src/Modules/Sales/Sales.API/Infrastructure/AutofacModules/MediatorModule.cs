﻿using Autofac;
using MediatR;

using VShop.SharedKernel.Application.Commands.Decorators;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.DomainEventHandlers;

namespace VShop.Modules.Sales.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register IMediator
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
                   .AsImplementedInterfaces();
                   
            builder.Register<ServiceFactory>(ctx =>
            {
                IComponentContext c = ctx.Resolve<IComponentContext>();
                return t => c.Resolve(t);
            });

            // Register command handlers
            builder.RegisterAssemblyTypes(typeof(CreateShoppingCartCommand).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register domain event handlers
            builder.RegisterAssemblyTypes(typeof(PlaceOrderDomainEventHandler).Assembly)
                   .AsClosedTypesOf(typeof(INotificationHandler<>));

            builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}