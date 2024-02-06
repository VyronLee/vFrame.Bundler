// ------------------------------------------------------------
//         File: ProfilerViewBase.cs
//        Brief: ProfilerViewBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-1 17:53
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

#if UNITY_2019_1_OR_NEWER

namespace vFrame.Bundler
{
    internal abstract class ProfilerViewBase<T> : ViewBase<ProfilerContexts, T> where T : class
    {
        protected ProfilerViewBase(ProfilerContexts contexts, string uxmlPath)
            : base(contexts, ProfilerAssetLocator.LocatorDir + uxmlPath) {

        }
    }
}

#endif