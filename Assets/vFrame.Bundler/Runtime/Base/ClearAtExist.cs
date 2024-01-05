// ------------------------------------------------------------
//         File: ClearAtExist.cs
//        Brief: ClearAtExist.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 16:52
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System;
using System.Collections;

namespace vFrame.Bundler
{
    internal readonly struct ClearAtExist : IDisposable
    {
        private readonly IList _collection;

        public ClearAtExist(IList collection) {
            _collection = collection;
        }

        public void Dispose() {
            _collection.Clear();
        }
    }
}