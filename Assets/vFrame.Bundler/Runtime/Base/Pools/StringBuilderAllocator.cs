// ------------------------------------------------------------
//         File: StringBuilderAllocator.cs
//        Brief: Allocator that creates StringBuilder instances with a fixed capacity and clears them for reuse.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:01
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Text;

namespace vFrame.Bundler
{
    internal class StringBuilderAllocator : IPoolObjectAllocator<StringBuilder>
    {
        public static int BuilderLength = 1024;

        public StringBuilder Alloc()
        {
            return new StringBuilder(BuilderLength);
        }

        public void Reset(StringBuilder obj)
        {
            obj.Length = 0;
        }
    }
}