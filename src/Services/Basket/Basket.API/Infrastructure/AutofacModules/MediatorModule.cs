using Autofac;
using MediatR; // TODO - review all MediatR references

using VShop.SharedKernel.Infrastructure.Decorators;
using VShop.Services.Basket.API.Application.Commands;

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

            builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}