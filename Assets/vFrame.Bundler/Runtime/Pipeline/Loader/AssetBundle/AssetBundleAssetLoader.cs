// ------------------------------------------------------------
//         File: AssetBundleAssetLoader.cs
//        Brief: AssetBundleAssetLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:10
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal abstract class AssetBundleAssetLoader : AssetLoader
    {
        protected AssetBundleLoaderGroup BundleLoader { get; }

        protected AssetBundleAssetLoader(BundlerContexts bundlerContexts,
            LoaderContexts loaderContexts,
            AssetBundleLoaderGroup bundleLoader) : base(bundlerContexts, loaderContexts) {

            BundleLoader = bundleLoader;
        }

    }
}