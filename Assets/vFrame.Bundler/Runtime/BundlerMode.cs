// ------------------------------------------------------------
//         File: BundlerMode.cs
//        Brief: Selects the underlying asset loading backend used by a Bundler instance.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 04:55:11
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Selects the underlying asset loading backend used by a Bundler instance.
    /// </summary>
    public enum BundlerMode
    {
        /// <summary>
        ///     Load assets directly via the Unity AssetDatabase. Editor-only.
        /// </summary>
        AssetDatabase,

        /// <summary>
        ///     Load assets from Unity's built-in Resources system.
        /// </summary>
        Resources,

        /// <summary>
        ///     Load assets from built AssetBundles; requires a valid BundlerManifest.
        /// </summary>
        AssetBundle
    }
}