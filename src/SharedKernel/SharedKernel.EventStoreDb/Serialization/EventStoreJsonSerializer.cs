using System;
using System.Text;
using Newtonsoft.Json;

using VShop.SharedKernel.EventStoreDb.Serialization.Contracts;

namespace VShop.SharedKernel.EventStoreDb.Serialization;

public class EventStoreJsonSerializer : IEventStoreSerializer
{
    public string ContentType => "application/json";

    public T Deserialize<T>(byte[] data) where T : class
        => Deserialize(data, typeof(T)) as T;

    public object Deserialize(byte[] data, Type type)
    {
        string jsonData = Encoding.UTF8.GetString(data);
        return JsonConvert.DeserializeObject(jsonData, type);
    }

    public byte[] Serialize(object data)
    {
        string jsonData = JsonConvert.SerializeObject(data);
        return Encoding.UTF8.GetBytes(jsonData);
    }
}