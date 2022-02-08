using System;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

using VShop.SharedKernel.Infrastructure.Extensions;

namespace VShop.SharedKernel.Infrastructure.Modules;

public static class ModuleLoader
{
    public static IEnumerable<Module> LoadModules(IConfiguration configuration)
    {
        IList<Assembly> assemblies = LoadAssemblies(configuration);
        
        IList<Type> moduleTypes = assemblies
            .SelectMany(a => a.GetTypes())
            .Where(t => typeof(Module).IsAssignableFrom(t) && t != typeof(Module))
            .OrderBy(t => t.Name)
            .ToList();

        foreach (Type moduleType in moduleTypes)
            yield return (Module)Activator.CreateInstance(moduleType, assemblies);
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
            if (!file.Contains(Module.Prefix)) continue;
            
            string moduleName = file.Split(Module.Prefix)[1].Split(".")[0].ToPascalCase();
            bool enabled = configuration.GetValue<bool>($"{moduleName}:Module:Enabled");
            if (!enabled) disabledModules.Add(file);
        }
        
        foreach (string disabledModule in disabledModules) files.Remove(disabledModule);

        files.ForEach(f => assemblies.Add(AppDomain.CurrentDomain.Load(AssemblyName.GetAssemblyName(f))));

        return assemblies;
    }
}