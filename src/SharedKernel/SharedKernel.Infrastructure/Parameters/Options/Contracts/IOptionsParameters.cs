using System.Collections.Generic;

namespace VShop.SharedKernel.Infrastructure.Parameters.Options.Contracts
{
    public interface IOptionsParameters
    {
        IReadOnlyList<string> Include { get; }
    }
}