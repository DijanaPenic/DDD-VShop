using System;
using System.Threading.Tasks;
using Microsoft.Extensions.Primitives;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

using VShop.SharedKernel.Infrastructure.Types;

namespace VShop.SharedKernel.Application.Providers
{
    public class GuidModelBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context is null) throw new ArgumentNullException(nameof(context));
            
            if (context.Metadata.ModelType != typeof(Guid)) return null;

            return context.BindingInfo.BindingSource == BindingSource.Header 
                ? new BinderTypeModelBinder(typeof(GuidHeaderModelBinder)) 
                : new BinderTypeModelBinder(typeof(GuidEntityModelBinder));
        }
    }
    
    public class GuidEntityModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null) throw new ArgumentNullException(nameof(bindingContext));
            if (bindingContext.ModelType != typeof(Guid)) return Task.CompletedTask;

            string modelName = bindingContext.ModelName;
            
            // Try to fetch the value of the argument by name
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None) return Task.CompletedTask;

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string value = valueProviderResult.FirstValue;
            
            if (Guid.TryParse(value, out Guid guidId))
            {
                bindingContext.Result = SequentialGuid.IsNullOrEmpty(guidId)
                    ? ModelBindingResult.Failed()
                    : ModelBindingResult.Success(guidId);
            }
            else bindingContext.ModelState.TryAddModelError(modelName, "Invalid format.");
            
            return Task.CompletedTask;
        }
    }
    
    public class GuidHeaderModelBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext is null) throw new ArgumentNullException(nameof(bindingContext));
            
            // Read HTTP header.
            string headerName = bindingContext.FieldName;
            if (bindingContext.HttpContext.Request.Headers.ContainsKey(headerName))
            {
                StringValues headerValues = bindingContext.HttpContext.Request.Headers[headerName];
                if (headerValues == StringValues.Empty)
                {
                    bindingContext.ModelState.TryAddModelError(headerName, "Value not found in HTTP header.");
                }
                else
                {
                    // Value found in HTTP header.
                    string value = headerValues[0];
                    bindingContext.ModelState.SetModelValue(bindingContext.FieldName, headerValues, value);
                    
                    // Parse GUID.
                    if (Guid.TryParse(value, out Guid guidId))
                    {
                        bindingContext.Result = SequentialGuid.IsNullOrEmpty(guidId)
                            ? ModelBindingResult.Failed()
                            : ModelBindingResult.Success(guidId);
                    }
                    else bindingContext.ModelState.TryAddModelError(headerName, "Invalid format.");
                }
            }
            else
            {
                bindingContext.ModelState.TryAddModelError(headerName, "HTTP header not found.");
            }
            
            return Task.CompletedTask;
        }
    }
}