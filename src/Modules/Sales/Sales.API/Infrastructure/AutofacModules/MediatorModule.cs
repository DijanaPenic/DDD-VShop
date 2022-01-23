using MediatR;
using Autofac;

using VShop.SharedKernel.Application.Decorators;
using VShop.Modules.Sales.Core.Commands;
using VShop.Modules.Sales.Core.ProcessManagers;
using VShop.SharedKernel.Infrastructure.Commands.Publishing;
using VShop.SharedKernel.Infrastructure.Commands.Publishing.Contracts;
using VShop.SharedKernel.Infrastructure.Events.Publishing;
using VShop.SharedKernel.Infrastructure.Events.Publishing.Contracts;

namespace VShop.Modules.Sales.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register IMediator
            builder.RegisterAssemblyTypes(typeof(IMediator).Assembly).AsImplementedInterfaces();
                   
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
            builder.RegisterAssemblyTypes(typeof(CreateShoppingCartCommandHandler).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register event handlers
            builder.RegisterAssemblyTypes(typeof(OrderingProcessManagerHandler).Assembly)
                   .AsClosedTypesOf(typeof(INotificationHandler<>));
            
            // Register behaviors
            builder.RegisterGeneric(typeof(RetryPolicyCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}