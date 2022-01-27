using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModule
{
    string Name { get; }
    Assembly[] Assemblies { get; set; }
    void Initialize(IConfiguration configuration, ILogger logger);
    void ConfigureCompositionRoot(IConfiguration configuration, ILogger logger);
}