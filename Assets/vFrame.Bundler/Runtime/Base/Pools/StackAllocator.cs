// ------------------------------------------------------------
//         File: StackAllocator.cs
//        Brief: Allocator that creates and resets Stack<T> instances for the object pool.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:50
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class StackAllocator<T> : IPoolObjectAllocator<Stack<T>>
    {
        public Stack<T> Alloc()
        {
            return new Stack<T>();
        }

        public void Reset(Stack<T> obj)
        {
            obj.Clear();
        }
    }
}