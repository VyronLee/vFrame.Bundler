// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupSync.cs
//        Brief: Synchronous AssetBundleLoaderGroup variant: creates sync child loaders and force-completes them
//               on start, because cached child loaders may still be in the middle of async loads.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:37:45
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    ///     <see cref="AssetBundleLoaderGroup"/> variant that loads through synchronous child loaders
    ///     (<see cref="AssetBundleLoaderSync"/>), guaranteeing the group finishes as soon as it starts.
    /// </summary>
    internal class AssetBundleLoaderGroupSync : AssetBundleLoaderGroup
    {
        /// <summary>
        ///     Initializes a new synchronous loader group for the target asset.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler contexts (manifest, loader registry, facade systems).</param>
        /// <param name="loaderContexts">Per-loader contexts (target asset path, parent loader, etc.).</param>
        public AssetBundleLoaderGroupSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts)
        {

        }

        /// <summary>Creates a synchronous child loader for the given bundle path.</summary>
        /// <param name="bundlePath">Bundle path resolved from the manifest.</param>
        /// <returns>A new <see cref="AssetBundleLoaderSync"/> instance.</returns>
        protected override AssetBundleLoader CreateAssetBundleLoader(string bundlePath)
        {
            return new AssetBundleLoaderSync(BundlerContexts, LoaderContexts, bundlePath);
        }

        /// <summary>
        ///     Starts all child loaders, then force-completes the group immediately.
        /// </summary>
        protected override void OnStart()
        {
            base.OnStart();
            // The loader cache may hand back shared loaders that are still loading asynchronously; force them to finish.
            ForceComplete();
        }
    }
}