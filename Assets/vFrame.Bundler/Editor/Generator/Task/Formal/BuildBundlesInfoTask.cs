// ------------------------------------------------------------
//         File: BuildBundlesInfoTask.cs
//        Brief: Formal build step 4: merges main and dependency asset infos into the final
//               bundle-path-to-asset-paths BundleInfos mapping.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:43:57
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Formal build step 4. Merges main and dependency asset infos into the final
    ///     <see cref="BuildContext.BundleInfos" /> mapping, associating each bundle path with all asset paths
    ///     packed into it.
    /// </summary>
    internal class BuildBundlesInfoTask : BuildTaskBase
    {
        /// <inheritdoc cref="BuildTaskBase.Run" />
        public override void Run(BuildContext context)
        {
            GenerateMainBundlesInfo(context, context.BundleInfos);
            GenerateDependencyBundlesInfo(context, context.BundleInfos);
        }

        /// <summary>
        ///     Adds each analyzed main asset's path to the bundle info of the bundle it was packed into, creating
        ///     the bundle info entry if it does not exist yet.
        /// </summary>
        /// <param name="context">The build context providing the analyzed main asset infos.</param>
        /// <param name="bundlesInfo">The shared bundle-path-to-info mapping to populate.</param>
        private void GenerateMainBundlesInfo(BuildContext context, IDictionary<string, BundleInfo> bundlesInfo)
        {
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                if (!bundlesInfo.TryGetValue(assetInfo.BundlePath, out var bundleInfo)) {
                    bundleInfo = bundlesInfo[assetInfo.BundlePath] = new BundleInfo { BundlePath = assetInfo.BundlePath };
                }
                bundleInfo.AssetPaths.Add(assetInfo.AssetPath);
            }
        }

        /// <summary>
        ///     Adds each analyzed shared-dependency asset's path to the bundle info of the bundle it was packed
        ///     into, creating the bundle info entry if it does not exist yet.
        /// </summary>
        /// <param name="context">The build context providing the analyzed dependency asset infos.</param>
        /// <param name="bundlesInfo">The shared bundle-path-to-info mapping to populate.</param>
        private void GenerateDependencyBundlesInfo(BuildContext context, IDictionary<string, BundleInfo> bundlesInfo)
        {
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