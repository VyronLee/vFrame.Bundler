// ------------------------------------------------------------
//         File: ScriptablePipeline.cs
//        Brief: ScriptablePipeline.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:20
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Task;

namespace vFrame.Bundler.Pipeline
{
    internal class ScriptablePipeline : PipelineBase
    {
        protected override BuildTaskBase[] GetTasks() {
            return new BuildTaskBase[] {};
        }
    }
}