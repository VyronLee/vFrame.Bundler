// ------------------------------------------------------------
//         File: ProfilerViewBase.cs
//        Brief: Base class for profiler views;
//               resolves each view's UXML path against the profiler asset locator directory.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:18:32
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


#if UNITY_2019_1_OR_NEWER

namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Base class for profiler views. Prepends the profiler asset locator directory to the given
    /// relative UXML path before delegating construction to <see cref="ViewBase{T1,T2}"/>.
    /// </summary>
    /// <typeparam name="T">Type of the view data bound to this view.</typeparam>
    internal abstract class ProfilerViewBase<T> : ViewBase<ProfilerContexts, T> where T : class
    {
        /// <summary>
        /// Initializes the view with shared profiler contexts and a UXML path relative to
        /// <see cref="ProfilerAssetLocator.LocatorDir"/>.
        /// </summary>
        /// <param name="contexts">Shared contexts of the profiler views.</param>
        /// <param name="uxmlPath">UXML asset path relative to the profiler asset locator directory.</param>
        protected ProfilerViewBase(ProfilerContexts contexts, string uxmlPath)
            : base(contexts, ProfilerAssetLocator.LocatorDir + uxmlPath)
        {

        }
    }
}

#endif