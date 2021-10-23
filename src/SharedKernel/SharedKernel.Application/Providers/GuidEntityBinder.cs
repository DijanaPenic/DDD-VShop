using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace VShop.SharedKernel.Application.Providers
{
    public class GuidEntityBinder : IModelBinder
    {
        public Task BindModelAsync(ModelBindingContext bindingContext)
        {
            if (bindingContext == null)
            {
                throw new ArgumentNullException(nameof(bindingContext));
            }
            if (bindingContext.ModelType != typeof(Guid))
            {
                return Task.CompletedTask;
            }

            string modelName = bindingContext.ModelName;
            
            // Try to fetch the value of the argument by name
            ValueProviderResult valueProviderResult = bindingContext.ValueProvider.GetValue(modelName);
            if (valueProviderResult == ValueProviderResult.None)
            {
                return Task.CompletedTask;
            }

            bindingContext.ModelState.SetModelValue(modelName, valueProviderResult);
            string value = valueProviderResult.FirstValue;
            
            if (Guid.TryParse(value, out Guid guid) && guid != Guid.Empty)
            {
                bindingContext.Result = ModelBindingResult.Success(guid); 
                
                return Task.CompletedTask;
            }

            // Empty guid arguments result in model state errors
            bindingContext.ModelState.TryAddModelError(modelName, "Value cannot be empty.");
            
            return Task.CompletedTask;
        }
    }
}