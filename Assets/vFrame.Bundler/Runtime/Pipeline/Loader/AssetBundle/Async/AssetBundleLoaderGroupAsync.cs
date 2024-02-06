// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupAsync.cs
//        Brief: AssetBundleLoaderGroupAsync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 19:1
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal class AssetBundleLoaderGroupAsync : AssetBundleLoaderGroup
    {
        public AssetBundleLoaderGroupAsync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        protected override AssetBundleLoader CreateAssetBundleLoader(string bundlePath) {
            return new AssetBundleLoaderAsync(BundlerContexts, LoaderContexts, bundlePath);
        }
    }
}