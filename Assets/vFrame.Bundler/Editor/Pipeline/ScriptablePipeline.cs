// ------------------------------------------------------------
//         File: ScriptablePipeline.cs
//        Brief: ScriptablePipeline.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:20
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using vFrame.Bundler.Editor.Task;

namespace vFrame.Bundler.Editor.Pipeline
{
    internal class ScriptablePipeline : PipelineBase
    {
        protected override BuildTaskBase[] GetTasks() {
            return new BuildTaskBase[] {};
        }
    }
}