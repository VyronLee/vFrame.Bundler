//------------------------------------------------------------
//        File:  ListAllocator.cs
//       Brief:  ListAllocator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 19:34
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class ListAllocator<T> : IPoolObjectAllocator<List<T>>
    {
        private const int ListLength = 64;

        public List<T> Alloc() {
            return new List<T>(ListLength);
        }

        public void Reset(List<T> obj) {
            obj.Clear();
        }
    }
}