using System;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageRegistry
{
    string GetName<TMessage>();
    string GetName(Type type);
    Type GetType(string name);
    bool TryTransform(string typeName, object input, out object result);
}