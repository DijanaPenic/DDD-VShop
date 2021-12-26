using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Parameters.Options.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Options
{
    public class OptionsParameters : IOptionsParameters
    {
        public IReadOnlyList<string> Include { get; }

        public OptionsParameters(IReadOnlyList<string> include) => Include = include;
    }
}