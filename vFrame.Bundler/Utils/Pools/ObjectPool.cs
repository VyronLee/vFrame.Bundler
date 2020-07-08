//------------------------------------------------------------
//        File:  ObjectPool.cs
//       Brief:  ObjectPool
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 19:09
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Collections.Generic;
using vFrame.Bundler.Interface;

namespace vFrame.Bundler.Utils.Pools
{
    public abstract class ObjectPool<TClass, TAllocator>
        where TClass: class, new()
        where TAllocator: IPoolObjectAllocator<TClass>, new()
    {
        private const int Capacity = 128;
        private static readonly Stack<TClass> Objects;
        private static readonly TAllocator Allocator;

        static ObjectPool()
        {
            Objects = new Stack<TClass>(Capacity);
            Allocator = new TAllocator();

            for (var i = 0; i < Capacity; i++)
                Objects.Push(Allocator.Alloc());
        }

        public static TClass Get()
        {
            return Objects.Count > 0 ? Objects.Pop() : Allocator.Alloc();
        }

        public static void Return(TClass obj)
        {
            Allocator.Reset(obj);

            if (Objects.Contains(obj))
                return;
            Objects.Push(obj);
        }
    }

    public abstract class ObjectPool<TClass> where TClass : class, new()
    {
        private static readonly Stack<TClass> Objects = new Stack<TClass>();

        public static TClass Get() {
            return Objects.Count > 0 ? Objects.Pop() : new TClass();
        }

        public static void Return(TClass obj) {
            if (Objects.Contains(obj)) {
                return;
            }
            Objects.Push(obj);
        }
    }
}