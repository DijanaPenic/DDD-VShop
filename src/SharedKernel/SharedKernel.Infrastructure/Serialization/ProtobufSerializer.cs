using System;
using System.IO;
using Google.Protobuf;

namespace VShop.SharedKernel.Infrastructure.Serialization
{
    public static class ProtobufSerializer
    {
        public static TData FromByteArray<TData>(byte[] data) where TData : IMessage
            => (TData)FromByteArray(data, typeof(TData));
        
        public static IMessage FromByteArray(byte[] data, Type type)
        {
            IMessage message = (IMessage)Activator.CreateInstance(type);
            
            using MemoryStream stream = new(data);
            message.MergeFrom(stream);
            
            return message;
        }

        public static IMessage FromByteString(ByteString data, Type type)
        {
            IMessage message = (IMessage)Activator.CreateInstance(type);
            message.MergeFrom(data);
            
            return message;
        }
    }
}