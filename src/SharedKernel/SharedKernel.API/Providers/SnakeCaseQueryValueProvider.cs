using System.Globalization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ModelBinding;

using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.API.Providers
{
    public class SnakeCaseQueryValueProvider : QueryStringValueProvider
    {
        public SnakeCaseQueryValueProvider
        (
            BindingSource bindingSource,
            IQueryCollection values,
            CultureInfo culture
        ): base(bindingSource, values, culture) { }

        public override bool ContainsPrefix(string prefix) => base.ContainsPrefix(prefix.ToSnakeCase());
        public override ValueProviderResult GetValue(string key) => base.GetValue(key.ToSnakeCase());
    }
}