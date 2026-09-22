// ------------------------------------------------------------
//         File: AnalyzePackByAllDirectoriesRule.cs
//        Brief: PackByAllDirectories analyzer: assigns each matched asset to a bundle named after
//               its containing directory.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:44:01
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.IO;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Main-asset analyzer for <c>PackByAllDirectories</c> rules: assigns each matched asset
    ///     to the bundle named after its containing directory.
    /// </summary>
    internal class AnalyzePackByAllDirectoriesRule : FormalMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Iterates the rule's assets, routing assets handled by built-in analyzers to that
        ///     path and assigning each remaining asset to a bundle named after its directory.
        /// </summary>
        /// <param name="context">Build context used to resolve bundle paths and record asset info.</param>
        /// <param name="rule">Main bundle rule supplying the assets to analyze.</param>
        /// <returns>A sequence of (asset path, progress) tuples with progress in (0, 1].</returns>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;

            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                if (TryBuiltinAnalyzer(context, asset)) {
                    continue;
                }

                var dirName = Path.GetDirectoryName(asset);
                if (string.IsNullOrEmpty(dirName)) {
                    continue;
                }

                var bundle = context.BuildBundlePath(dirName);
                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = bundle
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}