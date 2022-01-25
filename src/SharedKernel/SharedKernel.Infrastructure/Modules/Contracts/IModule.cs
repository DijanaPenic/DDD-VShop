using System;
using System.Reflection;
using Serilog;
using Microsoft.Extensions.Configuration;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModule
{
    string Name { get; }
    Assembly[] Assemblies { get; set; }
    void Use(IConfiguration configuration, ILogger logger);
}