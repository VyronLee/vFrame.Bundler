//------------------------------------------------------------
//        File:  HashSetPool.cs
//       Brief:  HashSetPool
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2020-06-12 20:48
//   Copyright:  Copyright (c) 2020, VyronLee
//============================================================
using System.Collections.Generic;

namespace vFrame.Bundler.Utils.Pools
{
    public class HashSetPool<T> : ObjectPool<HashSet<T>, HashSetAllocator<T>>
    {

    }
}