// ------------------------------------------------------------
//         File: SimulationPipeline.cs
//        Brief: Editor-only pipeline generating the manifest via main-asset analysis, without real bundle files.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:19:37
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation pipeline for editor development: analyzes main assets and generates the bundler
    ///     manifest without building real AssetBundle files.
    /// </summary>
    internal class SimulationPipeline : PipelineBase
    {
        /// <summary>
        ///     Creates the simulation task sequence: analyze main assets, then build the bundler manifest.
        /// </summary>
        /// <returns>The ordered simulation tasks: main-asset analysis, then manifest generation.</returns>
        protected override BuildTaskBase[] GetTasks()
        {
            return new BuildTaskBase[] {
                new SimulationAnalyzeMainAssetsTask(),
                new SimulationBuildBundlerManifestTask()
            };
        }
    }
}