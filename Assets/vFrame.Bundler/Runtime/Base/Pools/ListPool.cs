// ------------------------------------------------------------
//         File: ListPool.cs
//        Brief: Shared object pool that recycles List<T> instances through ListAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:40:23
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Shared pool of <see cref="List{T}" /> instances, backed by <see cref="ListAllocator{T}" />.
    ///     Provides pooled lists via <c>Get</c> and returns them for reuse via <c>Return</c>.
    /// </summary>
    /// <typeparam name="T">Element type stored in the pooled lists.</typeparam>
    internal class ListPool<T> : ObjectPool<List<T>, ListAllocator<T>>
    {
    }
}