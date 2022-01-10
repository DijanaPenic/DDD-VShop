using System;
using System.Reflection;
using System.Collections.Generic;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace VShop.SharedKernel.Infrastructure.Serialization
{
    // TODO - remove; not needed.
    public class PropertyIgnoreContractResolver : DefaultContractResolver
    {
        private readonly IList<Type> _ignores;

        public PropertyIgnoreContractResolver() => _ignores = new List<Type>();

        public void Ignore(Type type)
        {
            if (!_ignores.Contains(type)) _ignores.Add(type);
        }
        
        protected override JsonProperty CreateProperty(MemberInfo member, MemberSerialization memberSerialization)
        {
            JsonProperty property = base.CreateProperty(member, memberSerialization);

            if (_ignores.Contains(property.DeclaringType))
            {
                property.ShouldSerialize = o => false;
                property.Ignored = true;
            }

            return property;
        }
    }
}