using System;

using VShop.SharedKernel.Infrastructure.Parameters.Options.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Options
{
    public class OptionsFactory
    {
        public static IOptionsParameters Create(string include)
            => new OptionsParameters(string.IsNullOrWhiteSpace(include) ? 
                Array.Empty<string>() : include.Split(','));
    }
}