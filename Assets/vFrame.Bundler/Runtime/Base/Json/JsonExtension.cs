// ------------------------------------------------------------
//         File: JsonExtension.cs
//        Brief: Extension methods over MiniJson: serialize objects (including IJsonSerializable types
//               and their annotated properties) to strings or JsonObject, parse JSON strings into
//               JsonObject/JsonList, and read typed values with a safe default fallback.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:33:53
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

namespace vFrame.Bundler
{
    /// <summary>
    /// Thrown when a JSON string is expected to contain a JSON object but deserializes to another JSON type.
    /// </summary>
    public class NotJsonObjectException : System.Exception
    {

    }

    /// <summary>
    /// Thrown when a JSON string is expected to contain a JSON list but deserializes to another JSON type.
    /// </summary>
    public class NotJsonListException : System.Exception
    {

    }

    /// <summary>
    /// Extension methods over MiniJson for serializing objects, parsing JSON strings,
    /// and reading typed values with a safe default fallback.
    /// </summary>
    public static class JsonExtension
    {
        /// <summary>
        /// Serializes a JSON object into its JSON string representation.
        /// </summary>
        /// <param name="serializable">The JSON object to serialize.</param>
        /// <returns>The JSON string representation of <paramref name="serializable"/>.</returns>
        public static string ToJsonString(this JsonObject serializable)
        {
            return Json.Serialize(serializable);
        }

        /// <summary>
        /// Serializes an arbitrary object into its JSON string representation.
        /// </summary>
        /// <param name="obj">The object to serialize.</param>
        /// <returns>The JSON string representation of <paramref name="obj"/>.</returns>
        public static string ToJsonString(this object obj)
        {
            return Json.Serialize(obj);
        }

        /// <summary>
        /// Deserializes a JSON string and returns it as a JSON object.
        /// </summary>
        /// <param name="jsonStr">The JSON string to parse.</param>
        /// <returns>The parsed <see cref="JsonObject"/>.</returns>
        /// <exception cref="NotJsonObjectException">The string does not represent a JSON object.</exception>
        public static JsonObject ToJsonObject(this string jsonStr)
        {
            var json = Json.Deserialize(jsonStr);
            if (json is JsonObject jsonObject) {
                return jsonObject;
            }
            throw new NotJsonObjectException();
        }

        /// <summary>
        /// Deserializes a JSON string and returns it as a JSON list.
        /// </summary>
        /// <param name="jsonStr">The JSON string to parse.</param>
        /// <returns>The parsed <see cref="JsonList"/>.</returns>
        /// <exception cref="NotJsonListException">The string does not represent a JSON list.</exception>
        public static JsonList ToJsonList(this string jsonStr)
        {
            var json = Json.Deserialize(jsonStr);
            if (json is JsonList jsonList) {
                return jsonList;
            }
            throw new NotJsonListException();
        }

        /// <summary>
        /// Converts a serializable object to JSON data and serializes it into a JSON string.
        /// </summary>
        /// <param name="serializable">The object to convert and serialize.</param>
        /// <returns>The JSON string representation of <paramref name="serializable"/>.</returns>
        public static string ToJsonString(this IJsonSerializable serializable)
        {
            return Json.Serialize(ToJsonData(serializable));
        }

        /// <summary>
        /// Converts a list into a JSON list, recursively converting
        /// <see cref="IJsonSerializable"/> items into JSON objects.
        /// </summary>
        /// <param name="list">The list to convert.</param>
        /// <returns>The converted <see cref="JsonList"/>.</returns>
        public static JsonList ParseFromList(this IList list)
        {
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

        /// <summary>
        /// Converts a dictionary into a JSON object, recursively converting
        /// <see cref="IJsonSerializable"/> values into JSON objects.
        /// </summary>
        /// <param name="dictionary">The dictionary to convert.</param>
        /// <returns>The converted <see cref="JsonObject"/>.</returns>
        public static JsonObject ParseFromDictionary(this IDictionary<string, object> dictionary)
        {
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

        /// <summary>
        /// Converts a serializable object into a JSON object containing its
        /// <see cref="JsonSerializableProperty"/>-annotated instance properties, plus a
        /// "@TypeName" entry recording the concrete type name.
        /// </summary>
        /// <param name="serializable">The object to convert.</param>
        /// <returns>The converted <see cref="JsonObject"/>.</returns>
        public static JsonObject ToJsonData(this IJsonSerializable serializable)
        {
            var serializableType = serializable.GetType();
            var properties = serializableType.GetInstanceProperties();

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

        /// <summary>
        /// Reads the value under <paramref name="key"/> and converts it to <typeparamref name="T"/>,
        /// returning <paramref name="defaultValue"/> when the key is missing, the value is null,
        /// or the conversion fails (the failure is logged, not thrown).
        /// </summary>
        /// <typeparam name="T">The target type of the value.</typeparam>
        /// <param name="jsonData">The JSON object to read from.</param>
        /// <param name="key">The key of the value to read.</param>
        /// <param name="defaultValue">The value returned when the read or conversion fails.</param>
        /// <returns>The converted value, or <paramref name="defaultValue"/>.</returns>
        public static T SafeGetValue<T>(this JsonObject jsonData, string key, T defaultValue = default(T))
        {
            if (jsonData.TryGetValue(key, out var value)) {
                try {
                    // Number deserialize from MiniJson will only convert to long or double.
                    switch (value) {
                        case null:
                            return defaultValue;
                        case long longValue:
                            return (T)Convert.ChangeType(longValue, typeof(T));
                        case double doubleValue:
                            return (T)Convert.ChangeType(doubleValue, typeof(T));
                    }
                    return (T)value;
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