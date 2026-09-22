// ------------------------------------------------------------
//         File: ClearAtExist.cs
//        Brief: Struct that clears a wrapped collection when disposed; used as a using-scope guard.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:29:52
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Collections;

namespace vFrame.Bundler
{
    /// <summary>
    ///     A disposable guard that clears the collection it wraps when disposed, typically in a using-scope.
    /// </summary>
    internal readonly struct ClearAtExist : IDisposable
    {
        /// <summary>The collection to clear on disposal.</summary>
        private readonly IList _collection;

        /// <summary>
        ///     Creates a guard that clears the specified collection when disposed.
        /// </summary>
        /// <param name="collection">The collection to clear on disposal.</param>
        public ClearAtExist(IList collection)
        {
            _collection = collection;
        }

        /// <summary>
        ///     Clears all elements from the wrapped collection.
        /// </summary>
        /// <exception cref="NotSupportedException">The collection is read-only or has a fixed size.</exception>
        public void Dispose()
        {
            _collection.Clear();
        }
    }
}