// ------------------------------------------------------------
//         File: BuiltinPipeline.cs
//        Brief: BuiltinPipeline.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:13
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Editor.Task;

namespace vFrame.Bundler.Editor.Pipeline
{
    internal class BuiltinPipeline : PipelineBase
    {
        protected override BuildTaskBase[] GetTasks() {
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