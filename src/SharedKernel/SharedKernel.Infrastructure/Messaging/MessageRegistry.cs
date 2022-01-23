using System;
using System.Collections.Generic;

using VShop.SharedKernel.Infrastructure.Messaging.Contracts;

namespace VShop.SharedKernel.Infrastructure.Messaging;

public class MessageRegistry : IMessageRegistry
{
    private readonly IDictionary<Type, string> _typeMap = new Dictionary<Type, string>();
    private readonly IDictionary<string, Type> _nameMap = new Dictionary<string, Type>();

    public void Add<TMessage>(string typeName) => Add(typeof(TMessage), typeName);
    private void Add(Type type, string typeName)
    {
        if (_typeMap.ContainsKey(type) || _nameMap.ContainsKey(typeName))
            throw new ArgumentException("Duplicate name or type.");

        _typeMap.Add(type, typeName);
        _nameMap.Add(typeName, type);
    }
    
    private bool TryGetByType(Type type, out string typeName) => _typeMap.TryGetValue(type, out typeName);
    private bool TryGetByName(string typeName, out Type type) => _nameMap.TryGetValue(typeName, out type);
    
    public string GetName(Type type) => TryGetByType(type, out string name) ? name : null;
    public Type GetType(string name) => TryGetByName(name, out Type type) ? type : null;
}