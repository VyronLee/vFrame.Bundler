// ------------------------------------------------------------
//         File: JsonExtension.cs
//        Brief: JsonExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-22 20:14
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

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

        public static string ToJsonString(this object obj) {
            return Json.Serialize(obj);
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

        public static JsonList ParseFromList(this IList list) {
            var jsonList = new JsonList();
            foreach (var item in list) {
                switch (item) {
                    case IJsonSerializable serializable:
                        jsonList.Add(ToJsonData(serializable));
                        break;
                    default:
                        jsonList.Add(item);
                        break;
                }
            }
            return jsonList;
        }

        public static JsonObject ParseFromDictionary(this IDictionary<string, object> dictionary) {
            var jsonObject = new JsonObject();
            foreach (var item in dictionary) {
                switch (item.Value) {
                    case IJsonSerializable serializable:
                        jsonObject.Add(item.Key, ToJsonData(serializable));
                        break;
                    default:
                        jsonObject.Add(item.Key, item.Value);
                        break;
                }
            }
            return jsonObject;
        }

        public static JsonObject ToJsonData(this IJsonSerializable serializable) {
            var serializableType = serializable.GetType();
            var properties = serializableType.GetProperties(PropertyBindingFlags);

            var jsonData = new JsonObject {
                ["@TypeName"] = serializableType.Name
            };
            foreach (var property in properties) {
                var attribute = property.GetCustomAttribute<JsonSerializableProperty>(true);
                if (null == attribute) {
                    continue;
                }
                var formatToString = attribute.FormatToString;
                var format = attribute.Format;
                var value = property.GetValue(serializable);
                var fieldName = property.Name;
                switch (value) {
                    case null:
                        break;
                    case IList list:
                        jsonData.Add(fieldName, ParseFromList(list));
                        break;
                    case IDictionary<string, object> dictionary:
                        jsonData.Add(fieldName, ParseFromDictionary(dictionary));
                        break;
                    case IJsonSerializable jsonSerializable:
                        jsonData.Add(fieldName, ToJsonData(jsonSerializable));
                        break;
                    case IFormattable formattable:
                        if (formatToString) {
                            jsonData.Add(fieldName, formattable.ToString(format, null));
                        }
                        else {
                            jsonData.Add(fieldName, formattable);
                        }
                        break;
                    default:
                        jsonData.Add(fieldName, value);
                        break;
                }
            }
            return jsonData;
        }

        public static T SafeGetValue<T>(this JsonObject jsonData, string key, T defaultValue = default(T)) {
            if (jsonData.TryGetValue(key, out var value)) {
                try {
                    // Number deserialize from MiniJson will only convert to long or double.
                    switch (value) {
                        case null:
                            return defaultValue;
                        case long longValue:
                            return (T) Convert.ChangeType(longValue, typeof(T));
                        case double doubleValue:
                            return (T) Convert.ChangeType(doubleValue, typeof(T));
                    }
                    return (T) value;
                }
                catch (InvalidCastException) {
                    Debug.LogErrorFormat("Cannot convert value: {0} from type: {1} to type: {2}",
                        value, value?.GetType(), typeof(T));
                    return defaultValue;
                }
            }
            return defaultValue;
        }
    }
}