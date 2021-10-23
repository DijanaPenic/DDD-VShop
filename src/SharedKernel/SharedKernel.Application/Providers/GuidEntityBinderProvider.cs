using System;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;

namespace VShop.SharedKernel.Application.Providers
{
    public class GuidEntityBinderProvider : IModelBinderProvider
    {
        public IModelBinder GetBinder(ModelBinderProviderContext context)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context));
            }

            return context.Metadata.ModelType == typeof(Guid) ? new BinderTypeModelBinder(typeof(GuidEntityBinder)) : null;
        }
    }
}