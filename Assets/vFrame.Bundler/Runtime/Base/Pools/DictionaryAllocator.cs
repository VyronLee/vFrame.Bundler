// ------------------------------------------------------------
//         File: DictionaryAllocator.cs
//        Brief: Creates and clears Dictionary instances on behalf of an object pool.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:34:02
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Pool object allocator that produces new <see cref="Dictionary{TKey,TValue}"/> instances
    ///     and resets them to an empty state when returned to the pool.
    /// </summary>
    /// <typeparam name="T1">The type of the dictionary keys.</typeparam>
    /// <typeparam name="T2">The type of the dictionary values.</typeparam>
    internal class DictionaryAllocator<T1, T2> : IPoolObjectAllocator<Dictionary<T1, T2>>
    {
        /// <summary>
        ///     Creates a new, empty dictionary instance for the pool.
        /// </summary>
        /// <returns>A newly allocated <see cref="Dictionary{T1,T2}"/> instance.</returns>
        public Dictionary<T1, T2> Alloc()
        {
            return new Dictionary<T1, T2>();
        }

        /// <summary>
        ///     Resets the dictionary to an empty state so it can be reused from the pool.
        /// </summary>
        /// <param name="obj">The dictionary instance being returned to the pool.</param>
        public void Reset(Dictionary<T1, T2> obj)
        {
            obj.Clear();
        }
    }
}