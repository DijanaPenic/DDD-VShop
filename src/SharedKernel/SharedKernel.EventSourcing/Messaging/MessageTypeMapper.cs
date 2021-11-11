using System;
using System.Linq;
using System.Collections.Concurrent;

namespace VShop.SharedKernel.EventSourcing.Messaging
{
    public static class MessageTypeMapper
    {
        private static readonly ConcurrentDictionary<Type, string> TypeNameMap = new();
        private static readonly ConcurrentDictionary<string, Type> TypeMap = new();

        public static void AddCustomMap<T>(string mappedEventTypeName)
            => AddCustomMap(typeof(T), mappedEventTypeName);

        public static void AddCustomMap(Type eventType, string mappedEventTypeName)
        {
            TypeNameMap.AddOrUpdate(eventType, mappedEventTypeName, (_, _) => mappedEventTypeName);
            TypeMap.AddOrUpdate(mappedEventTypeName, eventType, (_, _) => eventType);
        }

        public static string ToName<T>()
            => ToName(typeof(T));
        
        public static string ToName(Type eventType) => TypeNameMap.GetOrAdd(eventType, (_) =>
        {
            string eventTypeName = eventType.FullName!.Replace(".", "_");

            TypeMap.AddOrUpdate(eventTypeName, eventType, (_, _) => eventType);

            return eventTypeName;
        });

        public static Type ToType(string eventTypeName) => TypeMap.GetOrAdd(eventTypeName, (_) =>
        {
            Type type = GetFirstMatchingTypeFromCurrentDomainAssembly(eventTypeName.Replace("_", "."))!;

            if (type == null)
                throw new Exception($"Type map for '{eventTypeName}' wasn't found!");

            TypeNameMap.AddOrUpdate(type, eventTypeName, (_, _) => eventTypeName);

            return type;
        });

        private static Type GetFirstMatchingTypeFromCurrentDomainAssembly(string typeName)
            => AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes()
                    .Where(x => x.FullName == typeName || x.Name == typeName))
                .FirstOrDefault();
    }
}