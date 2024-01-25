// ------------------------------------------------------------
//         File: JsonExtension.cs
//        Brief: JsonExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 20:14
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Reflection;

namespace vFrame.Bundler
{
    public class NotJsonObjectException : System.Exception {

    }

    public class NotJsonListException : System.Exception {

    }

    public static class JsonExtension
    {
        private const BindingFlags PropertyBindingFlags = BindingFlags.Instance
                                                          | BindingFlags.Public
                                                          | BindingFlags.NonPublic
                                                          | BindingFlags.FlattenHierarchy;

        public static string ToJsonString(this JsonObject serializable) {
            return Json.Serialize(serializable);
        }

        public static JsonObject ToJsonObject(this string jsonStr) {
            var json = Json.Deserialize(jsonStr);
            if (json is JsonObject jsonObject) {
                return jsonObject;
            }
            throw new NotJsonObjectException();
        }

        public static JsonList ToJsonList(this string jsonStr) {
            var json = Json.Deserialize(jsonStr);
            if (json is JsonList jsonList) {
                return jsonList;
            }
            throw new NotJsonListException();
        }

        public static string ToJsonString(this IJsonSerializable serializable) {
            return Json.Serialize(ToJsonData(serializable));
        }

        public static JsonObject ToJsonData(this IJsonSerializable serializable) {
            var serializableType = serializable.GetType();
            var properties = serializableType.GetProperties(PropertyBindingFlags);

            var jsonData = new JsonObject {
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

        public static T SafeGetValue<T>(this JsonObject jsonData, string key, T defaultValue = default(T)) {
            if (jsonData.TryGetValue(key, out var value)) {
                return (T) value;
            }
            return defaultValue;
        }
    }
}