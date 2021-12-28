using Autofac;
using MediatR;

using VShop.SharedKernel.Messaging.Events.Publishing;
using VShop.SharedKernel.Messaging.Events.Publishing.Contracts;
using VShop.Modules.Catalog.API.Application.EventHandlers;

namespace VShop.Modules.Catalog.API.Infrastructure.AutofacModules
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

            // Register event handlers
            builder.RegisterAssemblyTypes(typeof(OrderPaidIntegrationEventHandler).Assembly)
                .AsClosedTypesOf(typeof(INotificationHandler<>));
        }
    }
}