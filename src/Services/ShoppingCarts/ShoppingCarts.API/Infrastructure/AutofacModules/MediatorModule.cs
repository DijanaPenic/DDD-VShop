using Autofac;
using MediatR;

using VShop.SharedKernel.Application.Decorators;
using VShop.Services.ShoppingCarts.API.Application.Commands;

namespace VShop.Services.ShoppingCarts.API.Infrastructure.AutofacModules
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

            // TODO - Register domain event handlers

            builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}