// ------------------------------------------------------------
//         File: ResourcesSceneLoaderAsync.cs
//        Brief: Async scene loader for Resources mode;
//               empty subclass of RuntimeSceneLoaderAsync acting as the mode marker.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:50:37
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     Asynchronous scene loader for Resources mode.
    ///     Inherits all behavior from <see cref="RuntimeSceneLoaderAsync"/>; this subclass exists only to mark the
    ///     Resources-mode variant so the loader factory can select it for <c>AssetLoadType</c> requests served
    ///     from the built-in Resources system.
    /// </summary>
    internal class ResourcesSceneLoaderAsync : RuntimeSceneLoaderAsync
    {
        /// <summary>
        ///     Initializes the loader with its shared and per-request contexts.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-wide contexts.</param>
        /// <param name="loaderContexts">Per-request contexts describing the scene to load.</param>
        protected ResourcesSceneLoaderAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }
    }
}