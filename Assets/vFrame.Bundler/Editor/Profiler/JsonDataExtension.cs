// ------------------------------------------------------------
//         File: JsonDataExtension.cs
//        Brief: JsonDataExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-24 22:20
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Profiler
{
    internal static class JsonDataExtension
    {
        public static T SafeGetValue<T>(this Dictionary<string, object> jsonData, string key, T defaultValue = default(T)) {
            if (jsonData.TryGetValue(key, out var value)) {
                return (T) value;
            }
            return defaultValue;
        }
    }
}