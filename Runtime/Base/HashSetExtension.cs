// ------------------------------------------------------------
//         File: HashSetExtension.cs
//        Brief: HashSetExtension.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-5 21:8
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal static class HashSetExtension
    {
        public static void AddRange<T>(this HashSet<T> set, IEnumerable<T> items) {
            foreach (var item in items) {
                set.Add(item);
            }
        }
    }
}