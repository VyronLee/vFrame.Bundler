// ------------------------------------------------------------
//         File: HashSetPool.cs
//        Brief: Pool that recycles HashSet<T> instances, creating them via HashSetAllocator<T>.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:37:03
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// Object pool that recycles <see cref="HashSet{T}"/> instances using <see cref="HashSetAllocator{T}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the elements stored in the pooled hash sets.</typeparam>
    internal class HashSetPool<T> : ObjectPool<HashSet<T>, HashSetAllocator<T>>
    {
    }
}