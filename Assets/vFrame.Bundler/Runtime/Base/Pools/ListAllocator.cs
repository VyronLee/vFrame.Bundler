// ------------------------------------------------------------
//         File: ListAllocator.cs
//        Brief: Allocator that creates and resets List<T> instances for the object pool.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:32
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class ListAllocator<T> : IPoolObjectAllocator<List<T>>
    {
        private const int ListLength = 64;

        public List<T> Alloc()
        {
            return new List<T>(ListLength);
        }

        public void Reset(List<T> obj)
        {
            obj.Clear();
        }
    }
}