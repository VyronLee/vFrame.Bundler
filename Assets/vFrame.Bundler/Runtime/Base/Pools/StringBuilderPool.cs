// ------------------------------------------------------------
//         File: StringBuilderPool.cs
//        Brief: Object pool for StringBuilder instances, backed by StringBuilderAllocator.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:06:06
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Text;

namespace vFrame.Bundler
{
    internal class StringBuilderPool : ObjectPool<StringBuilder, StringBuilderAllocator>
    {
    }
}