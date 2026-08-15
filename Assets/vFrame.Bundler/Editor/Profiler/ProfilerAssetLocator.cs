// ------------------------------------------------------------
//         File: ProfilerAssetLocator.cs
//        Brief: ScriptableObject marker used to locate the profiler UI's folder (for loading uxml/uss assets) at runtime.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-2-1 17:47
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================


using System.IO;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler
{
    [CreateAssetMenu(menuName = "vFrame/Bundler/Profiler Asset Locator")]
    internal class ProfilerAssetLocator : ScriptableObject
    {
        public static string LocatorDir {
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