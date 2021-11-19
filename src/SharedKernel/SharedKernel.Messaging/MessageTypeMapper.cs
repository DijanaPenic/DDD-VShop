using System;
using System.Linq;
using System.Collections.Concurrent;

namespace VShop.SharedKernel.Messaging
{
    public static class MessageTypeMapper
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNameMap = new();
        private static readonly ConcurrentDictionary<string, Type> TypeMap = new();

        public static void AddCustomMap<T>(string mappedMessageTypeName)
            => AddCustomMap(typeof(T), mappedMessageTypeName);

        public static void AddCustomMap(Type messageType, string mappedMessageTypeName)
        {
            TypeNameMap.AddOrUpdate(messageType, mappedMessageTypeName, (_, _) => mappedMessageTypeName);
            TypeMap.AddOrUpdate(mappedMessageTypeName, messageType, (_, _) => messageType);
        }

        public static string ToName<T>() => ToName(typeof(T));
        
        public static string ToName(Type messageType) => TypeNameMap.GetOrAdd(messageType, (_) =>
        {
            string messageTypeName = messageType.FullName?.Replace(".", "_");

            TypeMap.AddOrUpdate(messageTypeName, messageType, (_, _) => messageType);

            return messageTypeName;
        });

        public static Type ToType(string messageTypeName) => TypeMap.GetOrAdd(messageTypeName, (_) =>
        {
            Type type = GetFirstMatchingTypeFromCurrentDomainAssembly(messageTypeName.Replace("_", "."));

            if (type == null)
                throw new Exception($"Type map for '{messageTypeName}' wasn't found!");

            TypeNameMap.AddOrUpdate(type, messageTypeName, (_, _) => messageTypeName);

            return type;
        });

        private static Type GetFirstMatchingTypeFromCurrentDomainAssembly(string typeName)
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(t => t.FullName == typeName || t.Name == typeName))
                .FirstOrDefault();
    }
}