using System;

namespace VShop.SharedKernel.Infrastructure.Messaging.Contracts;

public interface IMessageRegistry
{
    string GetName(Type type);
    Type GetType(string name);
}