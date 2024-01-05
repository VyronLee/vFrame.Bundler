// ------------------------------------------------------------
//         File: SimulationPipeline.cs
//        Brief: SimulationPipeline.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-5 19:16
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Task;
using vFrame.Bundler.Task.Simulation;

namespace vFrame.Bundler.Pipeline
{
    internal class SimulationPipeline : PipelineBase
    {
        protected override BuildTaskBase[] GetTasks() {
            return new BuildTaskBase[] {
                new AnalyzeMainAssetsTask(),
                new BuildBundlerManifestTask()
            };
        }
    }
}