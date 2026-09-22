// ------------------------------------------------------------
//         File: ProfilerAssetLocator.cs
//        Brief: Marker asset used to locate the profiler UI's folder at runtime (for loading uxml/uss assets).
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:13:06
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.IO;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Marker <see cref="ScriptableObject"/> asset used to locate the profiler UI's folder at editor time,
    ///     so the profiler views can load their uxml/uss assets relative to it at runtime.
    /// </summary>
    [CreateAssetMenu(menuName = "vFrame/Bundler/Profiler Asset Locator")]
    internal class ProfilerAssetLocator : ScriptableObject
    {
        /// <summary>
        ///     Gets the directory containing the first <see cref="ProfilerAssetLocator"/> asset found in the
        ///     project, with a trailing directory separator, so profiler views can resolve their uxml/uss paths.
        /// </summary>
        /// <returns>
        ///     The profiler UI directory path ending with a directory separator,
        ///     or an empty string when no locator asset exists in the project.
        /// </returns>
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