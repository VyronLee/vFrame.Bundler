// ------------------------------------------------------------
//         File: AnalyzeDependencyAssetsTask.cs
//        Brief: Build step 2: collects the full dependency closure of each main-asset bundle, recording which
//               bundles reference each shared dependency asset.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:19:40
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.Linq;
using UnityEditor;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Formal build task that maps each main-asset bundle to its full dependency closure and records, for
    ///     every dependency asset that is not itself a main asset, the set of main-asset bundles referencing it.
    /// </summary>
    internal class AnalyzeDependencyAssetsTask : BuildTaskBase
    {
        /// <summary>
        ///     Executes the dependency-asset analysis: one bundle entry is created per distinct main-asset bundle
        ///     path, then referencing bundles are collected for all of their non-main dependency assets.
        /// </summary>
        /// <param name="context">Build context providing main-asset info and receiving dependency-asset info.</param>
        public override void Run(BuildContext context)
        {
            var bundleInfos = CreateMainAssetBundleInfosFromMainAssetInfos(context);
            AnalyzeDependencyAssets(context, bundleInfos);
        }

        /// <summary>
        ///     Groups all main-asset infos by their bundle path, producing one <see cref="BundleInfo" /> per
        ///     distinct main-asset bundle together with the asset paths assigned to it.
        /// </summary>
        /// <param name="context">Build context whose main-asset infos are grouped by bundle.</param>
        /// <returns>Main-asset bundles, one entry per distinct bundle path.</returns>
        private static List<BundleInfo> CreateMainAssetBundleInfosFromMainAssetInfos(BuildContext context)
        {
            var assetInfos = context.MainAssetInfos.Values;
            var bundleInfos = new Dictionary<string, BundleInfo>();
            foreach (var assetInfo in assetInfos) {
                var bundlePath = assetInfo.BundlePath;
                if (!bundleInfos.TryGetValue(bundlePath, out var bundleInfo)) {
                    bundleInfo = bundleInfos[bundlePath] = new BundleInfo { BundlePath = bundlePath };
                }
                bundleInfo.AssetPaths.Add(assetInfo.AssetPath);
            }
            return bundleInfos.Values.ToList();
        }

        /// <summary>
        ///     Expands each main-asset bundle's asset paths to its full dependency closure (via
        ///     <see cref="AssetHelper.GetAllDependencies" />) and registers every dependency that is not itself
        ///     a main asset in <see cref="BuildContext.DependencyAssetInfos" /> with its referencing bundles.
        ///     Displays an editor progress bar which is cleared when the analysis finishes, even on failure.
        /// </summary>
        /// <param name="context">Build context providing main-asset info and receiving dependency-asset info.</param>
        /// <param name="mainAssetBundleInfos">Main-asset bundles to analyze, one per distinct bundle path.</param>
        private void AnalyzeDependencyAssets(BuildContext context, List<BundleInfo> mainAssetBundleInfos)
        {
            try {
                var index = 0f;
                var total = mainAssetBundleInfos.Count;
                foreach (var bundleInfo in mainAssetBundleInfos) {
                    EditorUtility.DisplayProgressBar("Analyzing Dependency Assets",
                        bundleInfo.BundlePath, ++index / total);

                    var dependencies = AssetHelper.GetAllDependencies(bundleInfo.AssetPaths.ToArray());
                    foreach (var dependency in dependencies) {
                        // IMPORTANT: Skip if already mark as main asset
                        if (context.MainAssetInfos.ContainsKey(dependency)) {
                            continue;
                        }
                        if (!context.DependencyAssetInfos.TryGetValue(dependency, out var dependencyAssetInfo)) {
                            dependencyAssetInfo = context.DependencyAssetInfos[dependency] =
                                new DependencyAssetInfo { AssetPath = dependency };
                        }
                        dependencyAssetInfo.ReferenceBundles.Add(bundleInfo.BundlePath);
                    }
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }
    }
}