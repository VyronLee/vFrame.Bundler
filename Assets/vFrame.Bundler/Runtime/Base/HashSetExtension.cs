// ------------------------------------------------------------
//         File: HashSetExtension.cs
//        Brief: Extension helpers for HashSet<T>, adding bulk insertion of enumerable items.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:29:56
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Extension methods for <see cref="HashSet{T}" />.
    /// </summary>
    internal static class HashSetExtension
    {
        /// <summary>
        ///     Adds each item of <paramref name="items" /> to the set, skipping items it already contains.
        /// </summary>
        /// <param name="set">The set to insert into.</param>
        /// <param name="items">The items to insert.</param>
        /// <typeparam name="T">The type of items in the set.</typeparam>
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items)
        {
            foreach (var item in items) {
                set.Add(item);
            }
        }
    }
}