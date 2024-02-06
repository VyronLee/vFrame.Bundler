// ------------------------------------------------------------
//         File: AssetBundleLoader.cs
//        Brief: AssetBundleLoader.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2024-1-3 22:49
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using UnityEngine;

namespace vFrame.Bundler
{
    internal abstract class AssetBundleLoader : Loader
    {
        protected AssetBundleLoader(BundlerContexts bundlerContexts, LoaderContexts loaderContexts, string bundlePath)
            : base(bundlerContexts, loaderContexts) {

            BundlePath = bundlePath;
            Adapter = bundlerContexts.Options.AssetBundleCreateAdapter ??
                       new InternalAssetBundleCreateAdapter(bundlerContexts);
        }

        [JsonSerializableProperty]
        public string BundlePath { get; }
        protected IAssetBundleCreateAdapter Adapter { get; }

        public abstract AssetBundle AssetBundle { get; }

        public override string ToString() {
            return $"[@TypeName: {GetType().Name}, BundlePath: {BundlePath}, TaskState: {TaskState}, Progress: {100 * Progress:F2}%]";
        }
    }
}