// ------------------------------------------------------------
//         File: ListAllocator.cs
//        Brief: Object pool allocator that creates new List<T> instances and clears them for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:37:11
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Object pool allocator that produces pre-sized <see cref="List{T}"/> instances and resets them
    ///     by clearing all elements for subsequent reuse.
    /// </summary>
    /// <typeparam name="T">Element type held by the allocated lists.</typeparam>
    internal class ListAllocator<T> : IPoolObjectAllocator<List<T>>
    {
        /// <summary>Initial capacity of newly allocated lists.</summary>
        private const int ListLength = 64;

        /// <summary>
        ///     Creates a new list with the default initial capacity.
        /// </summary>
        /// <returns>A new empty <see cref="List{T}"/> instance.</returns>
        public List<T> Alloc()
        {
            return new List<T>(ListLength);
        }

        /// <summary>
        ///     Prepares a list for return to the pool by removing all of its elements.
        /// </summary>
        /// <param name="obj">The list instance to reset. Must not be null.</param>
        public void Reset(List<T> obj)
        {
            obj.Clear();
        }
    }
}