using Newtonsoft.Json;
using System.Collections.Generic;
using NodaTime.Serialization.JsonNet;

namespace VShop.SharedKernel.Infrastructure.Serialization
{
    public static class DefaultJsonSerializer
    {
        public static readonly JsonSerializerSettings Settings = new()
        {
            DateParseHandling = DateParseHandling.None,
            Converters = new List<JsonConverter>
            {
                NodaConverters.InstantConverter
            }
        };
    }
}