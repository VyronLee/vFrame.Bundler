// ------------------------------------------------------------
//         File: AssetBundleAssetLoader.cs
//        Brief: Base class for AssetBundle-mode asset loaders; requires a parent
//               AssetBundleLoaderGroup and exposes it as BundleLoader.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 05:25:16
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


namespace vFrame.Bundler
{
    /// <summary>
    /// Base class for asset loaders operating in AssetBundle mode.
    /// Validates that the parent pipeline loader is an <see cref="AssetBundleLoaderGroup"/>
    /// and exposes it to derived loaders as <see cref="BundleLoader"/>.
    /// </summary>
    internal abstract class AssetBundleAssetLoader : AssetLoader
    {
        /// <summary>
        /// The parent bundle loader group that owns the underlying AssetBundle
        /// serving this loader's asset.
        /// </summary>
        protected AssetBundleLoaderGroup BundleLoader { get; }

        /// <summary>
        /// Creates a new AssetBundle-mode asset loader.
        /// </summary>
        /// <param name="bundlerContexts">Shared bundler-level contexts.</param>
        /// <param name="loaderContexts">Loader contexts; <see cref="LoaderContexts.ParentLoader"/>
        /// must be an <see cref="AssetBundleLoaderGroup"/>.</param>
        /// <exception cref="BundleArgumentException">Thrown when the parent loader is not
        /// an <see cref="AssetBundleLoaderGroup"/>.</exception>
        protected AssetBundleAssetLoader(BundlerContexts bundlerContexts,
            LoaderContexts loaderContexts) : base(bundlerContexts, loaderContexts)
        {

            BundleLoader = loaderContexts.ParentLoader as AssetBundleLoaderGroup;
            if (null == BundleLoader) {
                throw new BundleArgumentException("Parent loader must be AssetBundleLoaderGroup, got: "
                    + (null != BundleLoader ? BundleLoader.GetType().Name : "null"));
            }
        }

        /// <summary>
        /// Returns a debug description including the bundle path, asset path,
        /// task state, and loading progress.
        /// </summary>
        /// <returns>A formatted string describing this loader's current state.</returns>
        public override string ToString()
        {
            return $"[@TypeName: {GetType().Name}, BundlePath: {BundleLoader?.MainBundleLoader?.BundlePath}, AssetPath: {AssetPath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}