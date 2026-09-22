// ------------------------------------------------------------
//         File: StackPool.cs
//        Brief: Shared pool that recycles Stack<T> instances via StackAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:40:36
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    /// <summary>
    /// Object pool for <see cref="Stack{T}"/> instances; stacks are created and reset by
    /// <see cref="StackAllocator{T}"/> so pooled stacks are handed out empty.
    /// </summary>
    /// <typeparam name="T">Element type stored in the pooled stacks.</typeparam>
    internal class StackPool<T> : ObjectPool<Stack<T>, StackAllocator<T>>
    {
    }
}