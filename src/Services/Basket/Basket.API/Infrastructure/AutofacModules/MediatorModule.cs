using Autofac;
using MediatR;

using VShop.Services.Basket.API.Application.Commands;
using VShop.Services.Basket.API.Application.Behaviors;

namespace VShop.Services.Basket.API.Infrastructure.AutofacModules
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
            builder.RegisterAssemblyTypes(typeof(CreateBasketCommand).Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // TODO - Register domain event handlers

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}