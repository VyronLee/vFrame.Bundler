// ------------------------------------------------------------
//         File: StackAllocator.cs
//        Brief: Pool object allocator that creates empty Stack<T> instances and resets them by clearing contents.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:40:32
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// Pool object allocator that produces <see cref="Stack{T}"/> instances for the object pool.
    /// </summary>
    /// <typeparam name="T">Element type stored in the allocated stacks.</typeparam>
    internal class StackAllocator<T> : IPoolObjectAllocator<Stack<T>>
    {
        /// <summary>
        /// Creates a new empty stack instance.
        /// </summary>
        /// <returns>A newly created, empty <see cref="Stack{T}"/>.</returns>
        public Stack<T> Alloc()
        {
            return new Stack<T>();
        }

        /// <summary>
        /// Resets the stack to a reusable state by removing all its elements.
        /// </summary>
        /// <param name="obj">The stack instance to reset.</param>
        public void Reset(Stack<T> obj)
        {
            obj.Clear();
        }
    }
}