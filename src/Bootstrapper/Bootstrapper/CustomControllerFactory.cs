using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.Identity.Infrastructure.Configuration;

namespace VShop.Bootstrapper;

internal class CustomControllerFactory : IControllerFactory
{
    public object CreateController(ControllerContext context)
    {
        if (context is null) throw new ArgumentNullException(nameof(context));

        string assembly = context.ActionDescriptor.ControllerTypeInfo.AssemblyQualifiedName 
                          ?? throw new Exception("AssemblyQualifiedName is missing.");

        IServiceProvider serviceProvider;

        if (assembly.StartsWith(SalesCompositionRoot.NamePrefix)) 
            serviceProvider = SalesCompositionRoot.ServiceProvider;
        
        else if (assembly.StartsWith(BillingCompositionRoot.NamePrefix)) 
            serviceProvider = BillingCompositionRoot.ServiceProvider;
        
        else if (assembly.StartsWith(CatalogCompositionRoot.NamePrefix)) 
            serviceProvider = CatalogCompositionRoot.ServiceProvider;
        
        else if (assembly.StartsWith(IdentityCompositionRoot.NamePrefix)) 
            serviceProvider = IdentityCompositionRoot.ServiceProvider;

        else throw new Exception("ServiceProvider is missing.");

        Type controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        object controller = ActivatorUtilities.CreateInstance(serviceProvider, controllerType);

        return controller;
    }

    public void ReleaseController(ControllerContext context, object controller) { }
} 