// ------------------------------------------------------------
//         File: ScriptablePipeline.cs
//        Brief: Pipeline variant that runs no tasks by default, serving as a template for
//               user-defined custom build workflows.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:17:19
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Scriptable pipeline with an empty task list. Intended as a starting point for custom build
    ///     workflows: derive from it, override <see cref="GetTasks"/> to supply the desired build tasks.
    /// </summary>
    internal class ScriptablePipeline : PipelineBase
    {
        /// <inheritdoc cref="PipelineBase.GetTasks"/>
        /// <returns>An empty array; the pipeline performs no build steps unless tasks are supplied by a derived class.</returns>
        protected override BuildTaskBase[] GetTasks()
        {
            return new BuildTaskBase[] { };
        }
    }
}