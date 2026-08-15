// ------------------------------------------------------------
//         File: HashSetAllocator.cs
//        Brief: Allocator that creates and resets HashSet<T> instances for the object pool.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:15
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class HashSetAllocator<T> : IPoolObjectAllocator<HashSet<T>>
    {
        public HashSet<T> Alloc()
        {
            return new HashSet<T>();
        }

        public void Reset(HashSet<T> obj)
        {
            obj.Clear();
        }
    }
}