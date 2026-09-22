// ------------------------------------------------------------
//         File: AnalyzePackBySingleFileRule.cs
//        Brief: Packs each asset matched by a PackBySingleFile rule into its own bundle named after the asset.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:49:06
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Main-asset analyzer for PackBySingleFile rules: assigns every asset matched by the rule to its own bundle
    ///     whose path is derived from the asset path.
    /// </summary>
    internal class AnalyzePackBySingleFileRule : FormalMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Packs each matched asset into a separate bundle, deferring non-buildable, shader and scene assets to the
        ///     builtin analyzers.
        /// </summary>
        /// <param name="context">Build context providing bundle-path helpers and the main-asset info collection.</param>
        /// <param name="rule">Main bundle rule being analyzed; its SearchPath selects the assets to pack.</param>
        /// <returns>Yields each asset path with its progress fraction (processed count / total).</returns>
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

                var bundle = context.BuildBundlePath(asset);
                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = bundle
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}