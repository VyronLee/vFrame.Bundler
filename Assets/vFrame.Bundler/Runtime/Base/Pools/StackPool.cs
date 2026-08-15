// ------------------------------------------------------------
//         File: StackPool.cs
//        Brief: Object pool for Stack<T> instances, backed by StackAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:55
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class StackPool<T> : ObjectPool<Stack<T>, StackAllocator<T>>
    {
    }
}