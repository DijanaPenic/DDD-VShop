using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Controllers;

using VShop.Modules.Sales.Infrastructure.Configuration;
using VShop.Modules.Billing.Infrastructure.Configuration;
using VShop.Modules.Catalog.Infrastructure.Configuration;
using VShop.Modules.Identity.Infrastructure.Configuration;

namespace VShop.Bootstrapper;

internal sealed class ServiceBasedControllerActivator : IControllerActivator
{
    public object Create(ControllerContext context)
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

        IServiceScope scope = serviceProvider.CreateScope();
        context.HttpContext.Items[typeof(IServiceScope)] = scope;
        
        Type controllerType = context.ActionDescriptor.ControllerTypeInfo.AsType();
        object controller = scope.ServiceProvider.GetRequiredService(controllerType);
        
        return controller;
    }

    public void Release(ControllerContext context, object controller)
    {
        IDisposable disposable = (IDisposable)context.HttpContext.Items[typeof(IServiceScope)];
        disposable?.Dispose();
    }
}