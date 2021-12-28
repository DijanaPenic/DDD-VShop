using MediatR;
using Autofac;

using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.SharedKernel.Messaging.Commands.Publishing;
using VShop.SharedKernel.Messaging.Commands.Publishing.Contracts;
using VShop.Modules.Billing.API.Application.Commands;
using VShop.Modules.Billing.API.Application.EventHandlers;

namespace VShop.Modules.Billing.API.Infrastructure.AutofacModules
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
            
            // Register event bus
            builder.RegisterType<EventBus>().As<IEventBus>().SingleInstance();

            // Register command bus
            builder.RegisterType<CommandBus>().As<ICommandBus>().SingleInstance();

            // Register command handlers
            builder.RegisterAssemblyTypes(typeof(InitiateTransferCommand).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register domain event handlers
            builder.RegisterAssemblyTypes(typeof(OrderStockConfirmedIntegrationEventHandler).Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register behaviors
            builder.RegisterGeneric(typeof(ErrorCommandDecorator<>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(RetryPolicyCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}