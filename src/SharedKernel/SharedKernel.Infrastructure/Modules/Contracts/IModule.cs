using Serilog;
using System.Reflection;
using Microsoft.Extensions.Configuration;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModule
{
    string Name { get; }
    Assembly[] Assemblies { get; set; }
    void Add(IConfiguration configuration, ILogger logger);
}