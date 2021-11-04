using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.EventSourcing.Messaging
{
    public static class MessageTypeMapper
    {
        private static readonly Dictionary<Type, string> NamesByType = new();
        private static readonly Dictionary<string, Type> TypesByName = new();

        private static void Map(Type type, string name = null)
        {
            if (string.IsNullOrWhiteSpace(name))
                name = type.FullName;

            if (TypesByName.ContainsKey(name))
                throw new InvalidOperationException($"'{type}' is already mapped to the following name: {TypesByName[name]}");

            TypesByName[name] = type;
            NamesByType[type] = name;
        }

        private static bool TryGetType(string name, out Type type) => TypesByName.TryGetValue(name, out type);

        private static bool TryGetTypeName(Type type, out string name) => NamesByType.TryGetValue(type, out name);

        public static void Map<T>(string name) => Map(typeof(T), name);

        public static string ToName(Type type)
        {
            if (!TryGetTypeName(type, out string name))
                throw new Exception($"Failed to find name mapped with '{type}'."); 

            return name;
        }

        public static Type ToType(string name)
        {
            if (!TryGetType(name, out Type type))
                throw new Exception($"Failed to find type mapped with '{name}'.");

            return type;
        }
    }
}