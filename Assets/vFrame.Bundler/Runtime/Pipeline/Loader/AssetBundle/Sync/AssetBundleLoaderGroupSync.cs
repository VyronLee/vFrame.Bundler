// ------------------------------------------------------------
//         File: AssetBundleLoaderGroupSync.cs
//        Brief: AssetBundleLoaderGroupSync.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:0
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

namespace vFrame.Bundler
{
    internal class AssetBundleLoaderGroupSync : AssetBundleLoaderGroup
    {
        public AssetBundleLoaderGroupSync(BundlerContexts bundlerContexts, LoaderContexts loaderContexts)
            : base(bundlerContexts, loaderContexts) {

        }

        protected override AssetBundleLoader CreateAssetBundleLoader(string bundlePath) {
            return new AssetBundleLoaderSync(BundlerContexts, LoaderContexts, bundlePath);
        }

        protected override void OnStart() {
            base.OnStart();
            // 因为BundlerLoader缓存的原因，可能组里会含有异步加载中的Loader，因此要强制完成一次
            ForceComplete();
        }
    }
}