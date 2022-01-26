using System.Linq;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Parameters.Options.Contracts;

namespace VShop.SharedKernel.Infrastructure.Parameters.Options
{
    internal class OptionsParameters : IOptionsParameters
    {
        public IReadOnlyList<string> Include { get; }

        public OptionsParameters(IEnumerable<string> include)
            => Include = include.Select(p => p.ToPascalCase()).ToList(); 
    }
}