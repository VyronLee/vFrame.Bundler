// ------------------------------------------------------------
//         File: StringBuilderPool.cs
//        Brief: Object pool that recycles StringBuilder instances, backed by StringBuilderAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:45:55
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Text;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Shared pool of <see cref="StringBuilder" /> instances, backed by <see cref="StringBuilderAllocator" />.
    ///     Provides pooled builders via <c>Get</c> and returns them for reuse via <c>Return</c>.
    /// </summary>
    internal class StringBuilderPool : ObjectPool<StringBuilder, StringBuilderAllocator>
    {
    }
}