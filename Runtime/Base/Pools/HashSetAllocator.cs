//------------------------------------------------------------
//        File:  HashSetAllocator.cs
//       Brief:  HashSetAllocator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2020-06-12 20:48
//   Copyright:  Copyright (c) 2020, VyronLee
//============================================================

using System.Collections.Generic;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Base.Pools
{
    internal class HashSetAllocator<T> : IPoolObjectAllocator<HashSet<T>>
    {
        public HashSet<T> Alloc() {
            return new HashSet<T>();
        }

        public void Reset(HashSet<T> obj) {
            obj.Clear();
        }
    }
}