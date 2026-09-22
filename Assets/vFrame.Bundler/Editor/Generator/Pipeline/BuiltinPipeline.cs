// ------------------------------------------------------------
//         File: BuiltinPipeline.cs
//        Brief: Formal AssetBundle build pipeline supplying the standard seven-task sequence, from
//               main-asset analysis to manifest generation.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:17:14
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Formal (built-in) AssetBundle build pipeline. Supplies the standard seven-task sequence, from
    ///     main-asset analysis to bundler-manifest generation, for <see cref="PipelineBase" /> to execute.
    /// </summary>
    internal class BuiltinPipeline : PipelineBase
    {
        /// <inheritdoc cref="PipelineBase.GetTasks"/>
        /// <returns>
        ///     The formal build tasks in fixed execution order: main-asset analysis, dependency-asset
        ///     analysis, dependency auto-grouping, bundles-info construction, AssetBundle build,
        ///     build-outcome validation and bundler-manifest generation.
        /// </returns>
        protected override BuildTaskBase[] GetTasks()
        {
            return new BuildTaskBase[] {
                new AnalyzeMainAssetsTask(),
                new AnalyzeDependencyAssetsTask(),
                new AutoGroupingDependenciesTask(),
                new BuildBundlesInfoTask(),
                new BuildAssetBundleTask(),
                new ValidateBuildOutcomesTask(),
                new BuildBundlerManifestTask(),
            };
        }
    }
}