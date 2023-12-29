// ------------------------------------------------------------
//         File: AnalyzeDependencyAssetsTask.cs
//        Brief: AnalyzeDependencyAssetsTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:41
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using vFrame.Bundler.Editor.Helper;

namespace vFrame.Bundler.Editor.Task
{
    internal class AnalyzeDependencyAssetsTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            var bundleInfos = CreateMainAssetBundleInfosFromMainAssetInfos(context);
            AnalyzeDependencyAssets(context, bundleInfos);
        }

        private static List<BundleInfo> CreateMainAssetBundleInfosFromMainAssetInfos(BuildContext context) {
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

        private void AnalyzeDependencyAssets(BuildContext context, List<BundleInfo> mainAssetBundleInfos) {
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