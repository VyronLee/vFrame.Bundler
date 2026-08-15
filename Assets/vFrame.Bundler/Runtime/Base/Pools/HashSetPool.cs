// ------------------------------------------------------------
//         File: HashSetPool.cs
//        Brief: Object pool for HashSet<T> instances, backed by HashSetAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:21
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class HashSetPool<T> : ObjectPool<HashSet<T>, HashSetAllocator<T>>
    {
    }
}