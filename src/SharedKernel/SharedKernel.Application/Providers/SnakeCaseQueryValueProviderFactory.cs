using System.Globalization;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VShop.SharedKernel.Application.Providers
{
    public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));

            SnakeCaseQueryValueProvider valueProvider = new(
                BindingSource.Query,
                context.ActionContext.HttpContext.Request.Query,
                CultureInfo.CurrentCulture
            );

            context.ValueProviders.Add(valueProvider);

            return Task.CompletedTask;
        }
    }
}