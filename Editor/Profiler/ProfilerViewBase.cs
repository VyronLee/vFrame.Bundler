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

using System.IO;
using UnityEditor;

namespace vFrame.Bundler
{
    internal abstract class ProfilerViewBase<T> : ViewBase<ProfilerContexts, T> where T : class
    {
        protected ProfilerViewBase(ProfilerContexts contexts, string uxmlPath)
            : base(contexts, LocatorDir + uxmlPath) {

        }

        private static string LocatorDir {
            get {
                var locators = AssetDatabase.FindAssets($"t:{typeof(ProfilerAssetLocator)}");
                if (null == locators || locators.Length <= 0) {
                    return "";
                }
                var locatorPath = AssetDatabase.GUIDToAssetPath(locators[0]);
                var locatorDir = Path.GetDirectoryName(locatorPath);
                if (string.IsNullOrEmpty(locatorDir)) {
                    return "";
                }
                return locatorDir + Path.DirectorySeparatorChar;
            }
        }
    }
}

#endif