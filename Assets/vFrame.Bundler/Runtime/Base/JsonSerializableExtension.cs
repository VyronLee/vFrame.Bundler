// ------------------------------------------------------------
//         File: JsonSerializableExtension.cs
//        Brief: JsonSerializableExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 20:14
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Reflection;

namespace vFrame.Bundler
{
    internal static class JsonSerializableExtension
    {
        private const BindingFlags PropertyBindingFlags = BindingFlags.Instance
                                                          | BindingFlags.Public
                                                          | BindingFlags.NonPublic
                                                          | BindingFlags.FlattenHierarchy;

        public static string ToJsonString(this Dictionary<string, object> serializable) {
            return Json.Serialize(serializable);
        }

        public static string ToJsonString(this IJsonSerializable serializable) {
            return Json.Serialize(ToJsonData(serializable));
        }

        public static Dictionary<string, object> ToJsonData(this IJsonSerializable serializable) {
            var serializableType = serializable.GetType();
            var properties = serializableType.GetProperties(PropertyBindingFlags);

            var jsonData = new Dictionary<string, object> {
                ["@TypeName"] = serializableType.Name
            };
            foreach (var property in properties) {
                if (!property.IsDefined(typeof(JsonSerializableProperty), true)) {
                    continue;
                }
                var value = property.GetValue(serializable);
                var fieldName = property.Name;
                var fieldValue = value?.ToString() ?? "null";
                jsonData.Add(fieldName, fieldValue);
            }
            return jsonData;
        }
    }
}