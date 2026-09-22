// ------------------------------------------------------------
//         File: BuildTaskBase.cs
//        Brief: Abstract base for bundle build pipeline tasks; each task runs one build step
//               against the shared build context.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:37:18
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================



namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Abstract base for bundle build pipeline tasks. Each task performs one step of the bundle
    ///     build and reports its outcome through the shared <see cref="BuildContext"/>.
    /// </summary>
    internal abstract class BuildTaskBase
    {
        /// <summary>Executes the task's build step.</summary>
        /// <param name="context">Shared context carrying build inputs and accumulating step outputs.</param>
        public abstract void Run(BuildContext context);
    }
}