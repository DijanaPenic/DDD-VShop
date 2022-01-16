using System;
using System.Collections.Generic;

namespace VShop.SharedKernel.Messaging
{
    public static class MessageTransformations
    {
        private static readonly Dictionary<string, Func<object, object>> TransformationMap = new();

        public static bool TryTransform(string eventTypeName, object input, out object result)
        {
            if (!TransformationMap.TryGetValue(eventTypeName, out Func<object, object> transformObject))
            {
                result = null;
                return false;
            }

            result = transformObject(input);
            
            return true;
        }

        public static void Register<TOldMessage, TMessage>(Func<TOldMessage, TMessage> transformMessage)
            where TOldMessage : class
            where TMessage : class
        {
            TransformationMap.Add
            (
                typeof(TOldMessage).Name,
                input => transformMessage(input as TOldMessage)
            );
        }
    }
}