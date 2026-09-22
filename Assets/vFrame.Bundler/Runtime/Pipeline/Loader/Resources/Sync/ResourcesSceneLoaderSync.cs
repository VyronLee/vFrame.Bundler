// ------------------------------------------------------------
//         File: ResourcesSceneLoaderSync.cs
//        Brief: Resources-mode synchronous scene loader that inherits RuntimeSceneLoaderSync behavior as-is.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:55:54
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Synchronous scene loader for Resources mode; empty subclass of
    ///     <see cref="RuntimeSceneLoaderSync"/> adding no behavior of its own.
    /// </summary>
    internal class ResourcesSceneLoaderSync : RuntimeSceneLoaderSync
    {
        /// <summary>
        ///     Initializes the loader from the given bundler and request contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts passed to the base loader.</param>
        /// <param name="loaderContexts">Request context supplying the scene path and load mode.</param>
        protected ResourcesSceneLoaderSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}