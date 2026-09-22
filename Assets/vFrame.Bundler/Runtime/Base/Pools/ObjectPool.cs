// ------------------------------------------------------------
//         File: ObjectPool.cs
//        Brief: Static object pools that pre-allocate, hand out and recycle reusable class instances.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:40:28
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// Static object pool that recycles instances of <typeparamref name="TClass"/> through a custom allocator.
    /// A fixed number of instances is pre-allocated up front and returned instances are reset before reuse.
    /// Not thread-safe.
    /// </summary>
    /// <typeparam name="TClass">The pooled instance type.</typeparam>
    /// <typeparam name="TAllocator">Allocator type that creates and resets pooled instances.</typeparam>
    internal abstract class ObjectPool<TClass, TAllocator>
        where TClass : class, new()
        where TAllocator : IPoolObjectAllocator<TClass>, new()
    {
        /// <summary>Number of instances pre-allocated when the pool is initialized.</summary>
        private const int Capacity = 128;

        /// <summary>Recycled instances available for reuse.</summary>
        private static readonly Stack<TClass> Objects;

        /// <summary>Creates new instances and resets returned ones before they re-enter the pool.</summary>
        private static readonly TAllocator Allocator;

        /// <summary>
        /// Initializes the allocator and pre-allocates <see cref="Capacity"/> instances into the pool.
        /// </summary>
        static ObjectPool()
        {
            Objects = new Stack<TClass>(Capacity);
            Allocator = new TAllocator();

            for (var i = 0; i < Capacity; i++)
                Objects.Push(Allocator.Alloc());
        }

        /// <summary>
        /// Takes an instance from the pool, allocating a new one via the allocator when the pool is empty.
        /// </summary>
        /// <returns>A ready-to-use instance of <typeparamref name="TClass"/>.</returns>
        public static TClass Get()
        {
            return Objects.Count > 0 ? Objects.Pop() : Allocator.Alloc();
        }

        /// <summary>
        /// Resets the instance via the allocator and returns it to the pool for reuse.
        /// Instances already present in the pool are ignored.
        /// </summary>
        /// <param name="obj">The instance to return to the pool.</param>
        public static void Return(TClass obj)
        {
            Allocator.Reset(obj);

            if (Objects.Contains(obj))
                return;
            Objects.Push(obj);
        }
    }

    /// <summary>
    /// Minimal static object pool that creates instances with the parameterless constructor
    /// and recycles them without resetting. Not thread-safe.
    /// </summary>
    /// <typeparam name="TClass">The pooled instance type.</typeparam>
    public abstract class ObjectPool<TClass> where TClass : class, new()
    {
        /// <summary>Recycled instances available for reuse.</summary>
        private static readonly Stack<TClass> Objects = new Stack<TClass>();

        /// <summary>
        /// Takes an instance from the pool, creating a new one when the pool is empty.
        /// </summary>
        /// <returns>A ready-to-use instance of <typeparamref name="TClass"/>.</returns>
        public static TClass Get()
        {
            return Objects.Count > 0 ? Objects.Pop() : new TClass();
        }

        /// <summary>
        /// Returns the instance to the pool for reuse. Instances already present in the pool are ignored.
        /// </summary>
        /// <param name="obj">The instance to return to the pool.</param>
        public static void Return(TClass obj)
        {
            if (Objects.Contains(obj))
                return;
            Objects.Push(obj);
        }
    }
}