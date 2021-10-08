using Autofac;
using MediatR;
using System.Reflection;

using VShop.Services.Basket.API.Application.Commands;
using VShop.SharedKernel.Infrastructure.AppBehaviors;

namespace VShop.Services.Basket.API.Infrastructure.AutofacModules
{
    public class MediatorModule : Autofac.Module
    {
        protected override void Load(ContainerBuilder builder)
        {
            // Register IMediator
            builder.RegisterAssemblyTypes(typeof(IMediator).GetTypeInfo().Assembly)
                   .AsImplementedInterfaces();

            // Register command handlers
            builder.RegisterAssemblyTypes(typeof(CreateBasketCommand).GetTypeInfo().Assembly)
                   .AsClosedTypesOf(typeof(IRequestHandler<,>));

            // TODO - Register domain event handlers

            builder.RegisterGeneric(typeof(LoggingBehavior<,>)).As(typeof(IPipelineBehavior<,>));
        }
    }
}