// ------------------------------------------------------------
//         File: ListPool.cs
//        Brief: Object pool for List<T> instances, backed by ListAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:38
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class ListPool<T> : ObjectPool<List<T>, ListAllocator<T>>
    {
    }
}