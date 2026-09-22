// ------------------------------------------------------------
//         File: IPipeline.cs
//        Brief: Contract for a bundle build pipeline: executes a bundle build from the given rules and settings.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:29:37
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    /// Contract for a bundle build pipeline that produces AssetBundles and the bundler
    /// manifest from the given build rules and settings. Implemented by the builtin,
    /// simulation, and scriptable pipelines.
    /// </summary>
    public interface IPipeline
    {
        /// <summary>
        /// Runs the bundle build: analyzes main assets and dependencies, groups shared
        /// dependencies, builds the bundles, validates the outcomes, and generates the
        /// bundler manifest, according to <paramref name="buildRules"/> and
        /// <paramref name="buildSettings"/>.
        /// </summary>
        /// <param name="buildRules">Rules defining main-asset selection and shared-dependency grouping.</param>
        /// <param name="buildSettings">Settings controlling the build output, bundle naming, and build options.</param>
        void Build(BundleBuildRules buildRules, BundleBuildSettings buildSettings);
    }
}