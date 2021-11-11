using System;
using System.Linq;
using System.Collections.Concurrent;

namespace VShop.SharedKernel.EventSourcing.Messaging
{
    public class MessageTypeMapper
    {
        private static readonly MessageTypeMapper Instance = new();

        private readonly ConcurrentDictionary<Type, string> _typeNameMap = new();
        private readonly ConcurrentDictionary<string, Type> _typeMap = new();

        public static void AddCustomMap<T>(string mappedEventTypeName)
            => AddCustomMap(typeof(T), mappedEventTypeName);

        public static void AddCustomMap(Type eventType, string mappedEventTypeName)
        {
            Instance._typeNameMap.AddOrUpdate(eventType, mappedEventTypeName, (_, _) => mappedEventTypeName);
            Instance._typeMap.AddOrUpdate(mappedEventTypeName, eventType, (_, _) => eventType);
        }

        public static string ToName<TEventType>()
            => ToName(typeof(TEventType));

        public static string ToName(Type eventType) => Instance._typeNameMap.GetOrAdd(eventType, (_) =>
        {
            string eventTypeName = eventType.FullName!.Replace(".", "_");

            Instance._typeMap.AddOrUpdate(eventTypeName, eventType, (_, _) => eventType);

            return eventTypeName;
        });

        public static Type ToType(string eventTypeName) => Instance._typeMap.GetOrAdd(eventTypeName, (_) =>
        {
            Type type = GetFirstMatchingTypeFromCurrentDomainAssembly(eventTypeName.Replace("_", "."))!;

            if (type == null)
                throw new Exception($"Type map for '{eventTypeName}' wasn't found!");

            Instance._typeNameMap.AddOrUpdate(type, eventTypeName, (_, _) => eventTypeName);

            return type;
        });
        
        private static Type GetFirstMatchingTypeFromCurrentDomainAssembly(string typeName)
        {
            return AppDomain.CurrentDomain.GetAssemblies()
                .SelectMany(a => a.GetTypes().Where(x => x.FullName == typeName || x.Name == typeName))
                .FirstOrDefault();
        }
    }
}