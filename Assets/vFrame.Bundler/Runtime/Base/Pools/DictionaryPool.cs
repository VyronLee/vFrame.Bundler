// ------------------------------------------------------------
//         File: DictionaryPool.cs
//        Brief: Object pool for Dictionary<TKey,TValue> instances, backed by DictionaryAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:10
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class DictionaryPool<T1, T2> : ObjectPool<Dictionary<T1, T2>, DictionaryAllocator<T1, T2>>
    {
    }
}