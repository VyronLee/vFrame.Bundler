// ------------------------------------------------------------
//         File: DictionaryAllocator.cs
//        Brief: Allocator that creates and resets Dictionary<TKey,TValue> instances for the object pool.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:05
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class DictionaryAllocator<T1, T2> : IPoolObjectAllocator<Dictionary<T1, T2>>
    {
        public Dictionary<T1, T2> Alloc()
        {
            return new Dictionary<T1, T2>();
        }

        public void Reset(Dictionary<T1, T2> obj)
        {
            obj.Clear();
        }
    }
}