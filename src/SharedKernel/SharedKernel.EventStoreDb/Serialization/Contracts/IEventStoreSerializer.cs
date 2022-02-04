using System;

namespace VShop.SharedKernel.EventStoreDb.Serialization.Contracts;

using IProtoMessage = Google.Protobuf.IMessage;

public interface IEventStoreSerializer
{
    string ContentType { get; }
    T Deserialize<T>(byte[] data) where T : class;
    public object Deserialize(byte[] data, Type type);
    byte[] Serialize(object data);
}