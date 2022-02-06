using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using VShop.SharedKernel.Infrastructure.Extensions;
using VShop.SharedKernel.Infrastructure.Modules.Contracts;

namespace VShop.SharedKernel.Infrastructure.Modules;

public static class ModuleLoader
{
    private const string ModulePrefix = "VShop.Modules.";

    public static IList<IModule> LoadModules(IConfiguration configuration)
    {
        IList<Assembly> assemblyList = LoadAssemblies(configuration);
        
        IList<IModule> modules = assemblyList
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(IModule).IsAssignableFrom(t) && !t.IsInterface)
            .OrderBy(t => t.Name)
            .Select(Activator.CreateInstance)
            .Cast<IModule>()
            .ToList();

        foreach (IModule module in modules)
        {
            module.Assemblies = assemblyList
                .Where(a => a.FullName is not null && a.FullName.StartsWith($"{ModulePrefix}{module.Name}"))
                .ToArray();
        }

        return modules;
    }
    
    private static IList<Assembly> LoadAssemblies(IConfiguration configuration)
    {
        IList<Assembly> assemblies = AppDomain.CurrentDomain.GetAssemblies().ToList();
        string[] locations = assemblies.Where(a => !a.IsDynamic).Select(a => a.Location).ToArray();

        List<string> files = Directory.GetFiles(AppDomain.CurrentDomain.BaseDirectory, "*.dll")
            .Where(f => !locations.Contains(f, StringComparer.InvariantCultureIgnoreCase))
            .ToList();
        
        List<string> disabledModules = new();
        
        foreach (string file in files)
        {
            if (!file.Contains(ModulePrefix)) continue;
            
            string moduleName = file.Split(ModulePrefix)[1].Split(".")[0].ToPascalCase();
            bool enabled = configuration.GetValue<bool>($"{moduleName}:Module:Enabled");
            if (!enabled) disabledModules.Add(file);
        }
        
        foreach (string disabledModule in disabledModules) files.Remove(disabledModule);

        files.ForEach(f => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(f))));

        return assemblies;
    }
}