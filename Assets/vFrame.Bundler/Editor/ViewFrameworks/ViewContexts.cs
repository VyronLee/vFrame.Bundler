// ------------------------------------------------------------
//         File: ViewContexts.cs
//        Brief: Empty base context passed to every view via ViewBase; subclasses add shared state.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:25:41
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Empty base context object passed to every view. Provides no shared state by
    /// itself; subclasses add state shared across the views of one feature.
    /// </summary>
    internal class ViewContexts
    {

    }
}

#endif