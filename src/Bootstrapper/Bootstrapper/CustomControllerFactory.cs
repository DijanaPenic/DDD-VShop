using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.Modules.Sales.Infrastructure.Configuration;

namespace Bootstrapper;

internal class CustomControllerFactory : IControllerFactory
{
    public object CreateController(ControllerContext context)
    {
        if (context is null)  throw new ArgumentNullException(nameof(context));

        string assembly = context.ActionDescriptor.ControllerTypeInfo.AssemblyQualifiedName 
                          ?? throw new Exception("AssemblyQualifiedName is missing.");
        
        IServiceProvider serviceProvider;

        if (assembly.StartsWith(SalesCompositionRoot.NamePrefix)) serviceProvider = SalesCompositionRoot.ServiceProvider;
        else throw new Exception("ServiceProvider is missing.");

        Type controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        object controller = ActivatorUtilities.CreateInstance(serviceProvider, controllerType);

        return controller;
    }

    public void ReleaseController(ControllerContext context, object controller) { }
} 