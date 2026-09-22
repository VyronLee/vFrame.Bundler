// ------------------------------------------------------------
//         File: IAsync.cs
//        Brief: Coroutine-style asynchronous operation that tracks completion state and loading progress.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:30:01
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections;

namespace vFrame.Bundler
{
    /// <summary>
    ///     Coroutine-style asynchronous operation that can be yielded in a coroutine
    ///     until it completes, reporting its completion state and loading progress.
    /// </summary>
    public interface IAsync : IEnumerator
    {
        /// <summary>
        ///     Gets a value indicating whether the operation has completed.
        /// </summary>
        bool IsDone { get; }

        /// <summary>
        ///     Gets the normalized progress of the operation, ranging from 0 to 1.
        /// </summary>
        float Progress { get; }
    }
}