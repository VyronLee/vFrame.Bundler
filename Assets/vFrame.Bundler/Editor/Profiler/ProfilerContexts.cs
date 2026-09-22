// ------------------------------------------------------------
//         File: ProfilerContexts.cs
//        Brief: State-less view context shared by all profiler views; derives from ViewContexts.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:18:24
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Context passed to every profiler view. Provides no extra state;
    /// all profiler views share this single empty instance type.
    /// </summary>
    internal class ProfilerContexts : ViewContexts
    {

    }
}

#endif