// ------------------------------------------------------------
//         File: StringBuilderAllocator.cs
//        Brief: Object pool allocator that creates fixed-capacity StringBuilder instances and clears them for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:40:40
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Text;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Allocator for <see cref="StringBuilder" /> pools: creates instances with a fixed initial capacity
    ///     and resets them to empty for reuse.
    /// </summary>
    internal class StringBuilderAllocator : IPoolObjectAllocator<StringBuilder>
    {
        /// <summary>
        ///     Initial capacity assigned to every allocated <see cref="StringBuilder" />. Mutable so callers can
        ///     tune it before pooling begins; already-allocated instances keep their original capacity.
        /// </summary>
        public static int BuilderLength = 1024;

        /// <summary>
        ///     Creates a new <see cref="StringBuilder" /> with the capacity defined by <see cref="BuilderLength" />.
        /// </summary>
        /// <returns>A new empty <see cref="StringBuilder" /> ready for use by the pool.</returns>
        public StringBuilder Alloc()
        {
            return new StringBuilder(BuilderLength);
        }

        /// <summary>
        ///     Clears the builder's content so it can safely return to the pool; capacity is retained.
        /// </summary>
        /// <param name="obj">The <see cref="StringBuilder" /> to clear.</param>
        public void Reset(StringBuilder obj)
        {
            obj.Length = 0;
        }
    }
}