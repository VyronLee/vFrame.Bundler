// ------------------------------------------------------------
//         File: IAsync.cs
//        Brief: Coroutine-style asynchronous operation interface exposing IsDone and Progress.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-08-15 20:05:00
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.Collections;

namespace vFrame.Bundler
{
    public interface IAsync : IEnumerator
    {
        bool IsDone { get; }
        float Progress { get; }
    }
}