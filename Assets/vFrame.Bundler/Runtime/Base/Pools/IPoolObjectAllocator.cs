// ------------------------------------------------------------
//         File: IPoolObjectAllocator.cs
//        Brief: Contract for pool allocators: Alloc creates a new instance, Reset clears it for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:26
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    public interface IPoolObjectAllocator<T>
    {
        T Alloc();

        void Reset(T obj);
    }
}