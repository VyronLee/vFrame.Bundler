//------------------------------------------------------------
//        File:  FixedStringBuilderPool.cs
//       Brief:  FixedStringBuilderPool
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//     Modified:  2019-04-16 15:49
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System.Text;

namespace vFrame.Bundler
{
    internal class StringBuilderPool : ObjectPool<StringBuilder, StringBuilderAllocator>
    {
    }
}