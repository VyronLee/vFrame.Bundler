// ------------------------------------------------------------
//         File: TypeExtension.cs
//        Brief: Extension helpers for inspecting a type's instance properties across its
//               inheritance chain, deduplicated by signature (first declaration wins).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:50:57
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Extension methods for <see cref="Type" /> reflection inspection.
    /// </summary>
    internal static class TypeExtension
    {
        /// <summary>
        ///     Gets all instance properties declared by <paramref name="type" /> and every type in its
        ///     base-class chain, including private members. Properties overriding or hiding an inherited
        ///     one are deduplicated by "FullName Name" signature, keeping the most-derived declaration.
        /// </summary>
        /// <param name="type">The type to inspect. May not be null.</param>
        /// <returns>Instance properties from the whole inheritance chain, without signature duplicates.</returns>
        public static PropertyInfo[] GetInstanceProperties(this Type type)
        {
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