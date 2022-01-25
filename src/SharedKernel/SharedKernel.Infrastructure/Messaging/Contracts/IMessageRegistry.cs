using System;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageRegistry
{
    string GetName<T>();
    string GetName(Type type);
    Type GetType(string name);
}