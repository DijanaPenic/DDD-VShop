using System;
using System.IO;
using Google.Protobuf;

using IProtoMessage = Google.Protobuf.IMessage;

namespace VShop.SharedKernel.Infrastructure.Serialization
{
    public static class ProtobufSerializer
    {
        public static T FromByteArray<T>(byte[] data) where T : IProtoMessage
            => (T)FromByteArray(data, typeof(T));
        
        public static IProtoMessage FromByteArray(byte[] data, Type type)
        {
            IProtoMessage message = (IProtoMessage)Activator.CreateInstance(type);
            
            using MemoryStream stream = new(data);
            message.MergeFrom(stream);
            
            return message;
        }
        
        public static IProtoMessage FromByteString(ByteString data, Type type)
        {
            IProtoMessage message = (IProtoMessage)Activator.CreateInstance(type);
            message.MergeFrom(data);
            
            return message;
        }

        public static byte[] ToByteArray(IProtoMessage message) => message.ToByteArray();
    }
}