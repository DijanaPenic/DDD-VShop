using MediatR;
using Autofac;
using System.Security.Policy;

using VShop.SharedKernel.Application.Decorators;
using VShop.SharedKernel.Infrastructure.Messaging.Commands.Publishing;
using VShop.Modules.Sales.API.Application.Commands;
using VShop.Modules.Sales.API.Application.ProcessManagers;

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
            
            // Register Publisher
            builder.RegisterType(typeof(Publisher)).SingleInstance();

            // Register command handlers
            builder.RegisterAssemblyTypes(typeof(CreateShoppingCartCommand).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // Register domain event handlers
            builder.RegisterAssemblyTypes(typeof(OrderFulfillmentProcessManager).Assembly)
                   .AsClosedTypesOf(typeof(INotificationHandler<>));

            builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            
            builder.RegisterType<CommandBus>().As<ICommandBus>();
        }
    }
}