// ------------------------------------------------------------
//         File: AnalyzePackByAllFilesRule.cs
//        Brief: Packs every asset matched by a PackByAllFiles rule into one bundle named after the rule's SearchPath.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:44:05
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Main-asset analyzer for PackByAllFiles rules: assigns every asset matched by the rule to one shared bundle
    ///     whose path is derived from the rule's SearchPath.
    /// </summary>
    internal class AnalyzePackByAllFilesRule : FormalMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Assigns each matched asset to the rule's shared bundle, deferring non-buildable, shader and scene assets
        ///     to the builtin analyzers.
        /// </summary>
        /// <param name="context">Build context providing bundle-path helpers and the main-asset info collection.</param>
        /// <param name="rule">Main bundle rule being analyzed; its SearchPath names the target bundle.</param>
        /// <returns>Yields each asset path with its progress fraction (processed count / total).</returns>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var bundlePath = context.BuildBundlePath(rule.SearchPath);
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                if (TryBuiltinAnalyzer(context, asset)) {
                    continue;
                }

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = bundlePath
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}