// ------------------------------------------------------------
//         File: DictionaryPool.cs
//        Brief: Object pool that recycles Dictionary<TKey,TValue> instances, backed by DictionaryAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:36:54
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Object pool that recycles <see cref="Dictionary{TKey,TValue}"/> instances.
    ///     Instances are allocated by <see cref="DictionaryAllocator{T1,T2}"/> and reset to empty when returned.
    /// </summary>
    /// <typeparam name="T1">The type of the dictionary keys.</typeparam>
    /// <typeparam name="T2">The type of the dictionary values.</typeparam>
    internal class DictionaryPool<T1, T2> : ObjectPool<Dictionary<T1, T2>, DictionaryAllocator<T1, T2>>
    {
    }
}