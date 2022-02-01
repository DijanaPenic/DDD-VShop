using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;

using VShop.SharedKernel.Infrastructure.Contexts.Contracts;
using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModule
{
    string Name { get; }
    Assembly[] Assemblies { get; set; }

    void Initialize
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    );

    void ConfigureCompositionRoot
    (
        IConfiguration configuration,
        ILogger logger,
        IContextAccessor contextAccessor,
        IMessageContextRegistry messageContextRegistry
    );
}