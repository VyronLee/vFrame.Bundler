// ------------------------------------------------------------
//         File: TypeExtension.cs
//        Brief: TypeExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-5 20:55
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace vFrame.Bundler
{
    internal static class TypeExtension
    {
        public static PropertyInfo[] GetInstanceProperties(this Type type) {
            const BindingFlags bindingFlags = BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic |
                                              BindingFlags.DeclaredOnly;
            var uniqueProperties = new Dictionary<string, PropertyInfo>();

            do {
                var properties = type.GetProperties(bindingFlags);
                foreach (var property in properties) {
                    var propertySignature = $"{property.PropertyType.FullName} {property.Name}";
                    if (!uniqueProperties.ContainsKey(propertySignature)) {
                        uniqueProperties[propertySignature] = property;
                    }
                }
                type = type.BaseType;
            }
            while (type != null);

            return uniqueProperties.Values.ToArray();
        }
    }
}