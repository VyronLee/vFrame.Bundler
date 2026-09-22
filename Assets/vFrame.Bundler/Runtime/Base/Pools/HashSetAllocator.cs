// ------------------------------------------------------------
//         File: HashSetAllocator.cs
//        Brief: Pool allocator that creates empty HashSet<T> instances and clears them for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:36:59
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// Pool object allocator for <see cref="HashSet{T}"/> instances. Creates empty sets on allocation
    /// and clears existing sets so they can be safely reused by the object pool.
    /// </summary>
    /// <typeparam name="T">The type of elements stored in the allocated sets.</typeparam>
    internal class HashSetAllocator<T> : IPoolObjectAllocator<HashSet<T>>
    {
        /// <summary>
        /// Creates a new, empty <see cref="HashSet{T}"/> instance.
        /// </summary>
        /// <returns>A newly created empty set ready for use.</returns>
        public HashSet<T> Alloc()
        {
            return new HashSet<T>();
        }

        /// <summary>
        /// Resets the specified set for reuse by removing all of its elements.
        /// </summary>
        /// <param name="obj">The set to clear before returning it to the pool.</param>
        public void Reset(HashSet<T> obj)
        {
            obj.Clear();
        }
    }
}