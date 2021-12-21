using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using VShop.SharedKernel.Infrastructure;
using VShop.SharedKernel.Domain.ValueObjects;

namespace VShop.SharedKernel.Application.Providers
{
    public class GuidEntityBinder : IModelBinder
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
            
            if (Guid.TryParse(value, out Guid entityId))
            {
                Result<EntityId> result = EntityId.Create(entityId);

                if (result.IsError) bindingContext.ModelState.TryAddModelError(modelName, result.Error.ToString());
                else bindingContext.Result = ModelBindingResult.Success(entityId);
            }
            else bindingContext.ModelState.TryAddModelError(modelName, "Invalid format.");
            
            return Task.CompletedTask;
        }
    }
}