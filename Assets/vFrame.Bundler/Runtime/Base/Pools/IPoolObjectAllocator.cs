// ------------------------------------------------------------
//         File: IPoolObjectAllocator.cs
//        Brief: Contract for object pool allocators: Alloc creates a new instance, Reset clears an instance for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:37:07
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Contract for allocators used by object pools: creates new instances and clears returned ones for reuse.
    /// </summary>
    /// <typeparam name="T">Type of objects created and recycled by the allocator.</typeparam>
    public interface IPoolObjectAllocator<T>
    {
        /// <summary>
        ///     Creates a new instance of <typeparamref name="T" />.
        /// </summary>
        /// <returns>A newly created instance ready for use by the pool.</returns>
        T Alloc();

        /// <summary>
        ///     Clears the state of an instance so it can safely be reused.
        /// </summary>
        /// <param name="obj">The instance to clear before it returns to the pool.</param>
        void Reset(T obj);
    }
}