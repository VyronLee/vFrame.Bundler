//------------------------------------------------------------
//        File:  StringBuilderAllocator.cs
//       Brief:  StringBuilderAllocator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 19:27
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System.Text;

namespace vFrame.Bundler
{
    internal class StringBuilderAllocator : IPoolObjectAllocator<StringBuilder>
    {
        public static int BuilderLength = 1024;

        public StringBuilder Alloc() {
            return new StringBuilder(BuilderLength);
        }

        public void Reset(StringBuilder obj) {
            obj.Length = 0;
        }
    }
}