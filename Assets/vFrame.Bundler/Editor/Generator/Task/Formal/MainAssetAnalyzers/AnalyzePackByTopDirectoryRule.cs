// ------------------------------------------------------------
//         File: AnalyzePackByTopDirectoryRule.cs
//        Brief: PackByTopDirectory analyzer: packs assets directly under the rule's SearchPath into
//               one bundle named after it, skipping subdirectory assets.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:49:10
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.IO;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Main-asset analyzer for <c>PackByTopDirectory</c> rules: assigns each asset located directly
    ///     under the rule's SearchPath to one shared bundle named after that path, skipping deeper assets.
    /// </summary>
    internal class AnalyzePackByTopDirectoryRule : FormalMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Iterates the rule's assets, routing assets handled by built-in analyzers to that path and
        ///     assigning each remaining top-directory asset to the rule's shared bundle.
        /// </summary>
        /// <param name="context">Build context used to resolve bundle paths and record asset info.</param>
        /// <param name="rule">Main bundle rule being analyzed; its SearchPath names the target bundle.</param>
        /// <returns>A sequence of (asset path, progress) tuples with progress in (0, 1].</returns>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var bundlePath = context.BuildBundlePath(rule.SearchPath);
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                if (!IsTopDirectoryAsset(asset)) {
                    continue;
                }
                if (TryBuiltinAnalyzer(context, asset)) {
                    continue;
                }

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = bundlePath
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
            yield break;

            /// <summary>
            ///     Determines whether the asset resides directly in the rule's SearchPath (its parent directory
            ///     equals the SearchPath), meaning it belongs to the shared top-level bundle.
            /// </summary>
            /// <param name="path">Asset path to test.</param>
            /// <returns><c>true</c> if the asset's containing directory equals the rule's SearchPath.</returns>
            bool IsTopDirectoryAsset(string path)
            {
                var dirName = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dirName)) {
                    return false;
                }
                return dirName == rule.SearchPath;
            }
        }
    }
}