using Autofac;

namespace VShop.Modules.Catalog.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // // Register IMediator
            // builder.RegisterAssemblyTypes(typeof(IMediator).Assembly)
            //     .AsImplementedInterfaces();
            //        
            // builder.Register<ServiceFactory>(ctx =>
            // {
            //     IComponentContext c = ctx.Resolve<IComponentContext>();
            //     return t => c.Resolve(t);
            // });
            //
            // // Register command bus
            // builder.RegisterType<CommandBus>().As<ICommandBus>().SingleInstance();
            //
            // // Register command handlers
            // builder.RegisterAssemblyTypes(typeof(InitiatePaymentCommand).Assembly)
            //     .AsClosedTypesOf(typeof(IRequestHandler<,>));
            //
            // // Register behaviors
            // builder.RegisterGeneric(typeof(ErrorCommandDecorator<>)).As(typeof(IPipelineBehavior<,>));
            // builder.RegisterGeneric(typeof(ErrorCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            // builder.RegisterGeneric(typeof(RetryPolicyCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            // builder.RegisterGeneric(typeof(LoggingCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
            // builder.RegisterGeneric(typeof(TransactionCommandDecorator<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}