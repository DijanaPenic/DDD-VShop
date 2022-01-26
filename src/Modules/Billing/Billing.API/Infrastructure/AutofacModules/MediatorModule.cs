using MediatR;
using Autofac;

using VShop.SharedKernel.Application.Decorators;
using VShop.Modules.Billing.API.Application.Commands;
using VShop.Modules.Billing.API.Application.EventHandlers;
using VShop.SharedKernel.Infrastructure.Commands;
using VShop.SharedKernel.Infrastructure.Commands.Contracts;
using VShop.SharedKernel.Infrastructure.Events;
using VShop.SharedKernel.Infrastructure.Events.Contracts;

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
            builder.RegisterType<EventDispatcher>().As<IEventDispatcher>().SingleInstance();

            // Register command bus
            builder.RegisterType<CommandDispatcher>().As<ICommandDispatcher>().SingleInstance();

            // Register command handlers
            builder.RegisterAssemblyTypes(typeof(TransferCommand).Assembly)
                .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register event handlers
            builder.RegisterAssemblyTypes(typeof(OrderFinalizedIntegrationEventHandler).Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));

            // Register behaviors
            builder.RegisterGeneric(typeof(RetryPolicyCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(TransactionalCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}