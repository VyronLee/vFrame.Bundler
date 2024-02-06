//------------------------------------------------------------
//        File:  StackAllocator.cs
//       Brief:  StackAllocator
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-07-09 19:34
//   Copyright:  Copyright (c) 2024, VyronLee
//============================================================

using System.Collections.Generic;

namespace vFrame.Bundler
{
    internal class StackAllocator<T> : IPoolObjectAllocator<Stack<T>>
    {
        public Stack<T> Alloc() {
            return new Stack<T>();
        }

        public void Reset(Stack<T> obj) {
            obj.Clear();
        }
    }
}