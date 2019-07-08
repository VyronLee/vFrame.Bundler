//------------------------------------------------------------
//        File:  FixedStringBuilderPool.cs
//       Brief:  FixedStringBuilderPool
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//     Modified:  2019-04-16 15:49
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using System.Text;

namespace vBundler.Utils
{
    public static class FixedStringBuilderPool
    {
        private const int BuildersCount = 64;
        private const int BuilderLength = 1000;
        private static readonly Stack<StringBuilder> Builders;

        static FixedStringBuilderPool()
        {
            Builders = new Stack<StringBuilder>(BuildersCount);
            for (var i = 0; i < BuildersCount; i++)
                Builders.Push(new StringBuilder(BuilderLength));
        }

        public static StringBuilder Get()
        {
            return Builders.Count > 0 ? Builders.Pop() : new StringBuilder(BuilderLength);
        }

        public static void Return(StringBuilder sb)
        {
            sb.Length = 0;
            Builders.Push(sb);
        }
    }
}