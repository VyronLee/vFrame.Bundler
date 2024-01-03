// ------------------------------------------------------------
//         File: BuildBundlesInfoTask.cs
//        Brief: BuildBundlesInfoTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 20:37
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Editor.Task
{
    internal class BuildBundlesInfoTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            GenerateMainBundlesInfo(context, context.BundleInfos);
            GenerateDependencyBundlesInfo(context, context.BundleInfos);
        }

        private void GenerateMainBundlesInfo(BuildContext context, IDictionary<string, BundleInfo> bundlesInfo) {
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                if (!bundlesInfo.TryGetValue(assetInfo.BundlePath, out var bundleInfo)) {
                    bundleInfo = bundlesInfo[assetInfo.BundlePath] = new BundleInfo { BundlePath = assetInfo.BundlePath };
                }
                bundleInfo.AssetPaths.Add(assetInfo.AssetPath);
            }
        }

        private void GenerateDependencyBundlesInfo(BuildContext context, IDictionary<string, BundleInfo> bundlesInfo) {
            foreach (var kv in context.DependencyAssetInfos) {
                var assetInfo = kv.Value;
                if (!bundlesInfo.TryGetValue(assetInfo.BundlePath, out var bundleInfo)) {
                    bundleInfo = bundlesInfo[assetInfo.BundlePath] = new BundleInfo { BundlePath = assetInfo.BundlePath };
                }
                bundleInfo.AssetPaths.Add(assetInfo.AssetPath);
            }
        }
    }
}