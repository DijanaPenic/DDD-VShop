using System;

using VShop.SharedKernel.Infrastructure.Serialization;
using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;

using IProtoMessage = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.EventStoreDb.Serialization;

public class EventStoreProtobufSerializer : IEventStoreSerializer
{
    public string ContentType => "application/octet-stream";

    public T Deserialize<T>(byte[] data) where T : class
        => ProtobufSerializer.FromByteArray(data, typeof(T)) as T;

    public object Deserialize(byte[] data, Type type) 
        => ProtobufSerializer.FromByteArray(data, type);

    public byte[] Serialize(object data)
    {
        return data is not IProtoMessage proto 
            ? Array.Empty<byte>()
            : ProtobufSerializer.ToByteArray(proto);
    }
}