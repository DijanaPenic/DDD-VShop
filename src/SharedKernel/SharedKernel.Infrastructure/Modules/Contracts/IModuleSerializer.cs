using System;

namespace VShop.SharedKernel.Infrastructure.Modules.Contracts;

public interface IModuleSerializer
{
    byte[] Serialize<T>(T value);
    T Deserialize<T>(byte[] value);
    object Deserialize(byte[] value, Type type);
}