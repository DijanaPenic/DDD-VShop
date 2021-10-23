using System;
using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VShop.SharedKernel.Application.Providers
{
    public class SnakeCaseQueryValueProviderFactory : IValueProviderFactory
    {
        public Task CreateValueProviderAsync(ValueProviderFactoryContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

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