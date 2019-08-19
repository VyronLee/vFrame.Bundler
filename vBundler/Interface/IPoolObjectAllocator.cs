//------------------------------------------------------------
//        File:  IPoolObjectAllocator.cs
//       Brief:  IPoolObjectAllocator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 19:19
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================
namespace vBundler.Interface
{
    public interface IPoolObjectAllocator<T>
    {
        T Alloc();

        void Reset(T obj);
    }
}