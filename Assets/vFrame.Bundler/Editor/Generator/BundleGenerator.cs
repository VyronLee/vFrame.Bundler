// ------------------------------------------------------------
//         File: BundleGenerator.cs
//        Brief: Static entry points for creating the Builtin, Scriptable and Simulation AssetBundle build pipelines.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:17:23
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Static entry points for creating the AssetBundle build pipelines of the bundler.
    /// </summary>
    public static class BundleGenerator
    {
        /// <summary>
        ///     Gets a new <c>BuiltinPipeline</c> instance that performs the formal AssetBundle build.
        /// </summary>
        public static IPipeline BuiltinPipeline => new BuiltinPipeline();

        /// <summary>
        ///     Gets a new <c>ScriptablePipeline</c> instance for custom build workflows.
        ///     Note: the property name is a historical misspelling of "Scriptable".
        /// </summary>
        public static IPipeline ScriptalbePipeline => new ScriptablePipeline();

        /// <summary>
        ///     Gets a new <c>SimulationPipeline</c> instance that generates the bundler manifest
        ///     without producing real AssetBundle files.
        /// </summary>
        public static IPipeline SimulationPipeline => new SimulationPipeline();
    }
}