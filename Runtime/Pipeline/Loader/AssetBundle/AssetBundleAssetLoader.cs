// ------------------------------------------------------------
//         File: AssetBundleAssetLoader.cs
//        Brief: AssetBundleAssetLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 23:10
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using vFrame.Bundler.Exception;

namespace vFrame.Bundler
{
    internal abstract class AssetBundleAssetLoader : AssetLoader
    {
        protected AssetBundleLoaderGroup BundleLoader { get; }

        protected AssetBundleAssetLoader(BundlerContexts bundlerContexts,
            LoaderContexts loaderContexts) : base(bundlerContexts, loaderContexts) {

            BundleLoader = loaderContexts.ParentLoader as AssetBundleLoaderGroup;
            if (null == BundleLoader) {
                throw new BundleArgumentException("Parent loader must be AssetBundleLoaderGroup, got: "
                    + (null != BundleLoader ? BundleLoader.GetType().Name : "null"));
            }
        }

        public override string ToString() {
            return $"[Type: {GetType().Name}, BundlePath: {BundleLoader?.MainBundleLoader?.BundlePath}, AssetPath: {AssetPath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}