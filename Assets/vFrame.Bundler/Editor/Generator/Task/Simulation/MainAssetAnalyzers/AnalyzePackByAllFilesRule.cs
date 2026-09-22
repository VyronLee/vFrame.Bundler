// ------------------------------------------------------------
//         File: AnalyzePackByAllFilesRule.cs
//        Brief: Simulation analyzer: maps each PackByAllFiles-matched asset to the <AssetDatabase> placeholder bundle.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 03:58:04
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation main-asset analyzer for <c>PackByAllFiles</c> rules: assigns each asset matched by the rule
    ///     to the <c>&lt;AssetDatabase&gt;</c> placeholder bundle so that editor-mode loading resolves assets directly
    ///     through the AssetDatabase instead of built bundles.
    /// </summary>
    internal class SimulationAnalyzePackByAllFilesRule : SimulationMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Adds a <see cref="MainAssetInfo" /> entry for every asset matched by the rule, yielding progress
        ///     (matched asset path, normalized progress) after each entry.
        /// </summary>
        /// <param name="context">Build context that collects the generated main-asset info.</param>
        /// <param name="rule">Main bundle rule whose matched assets are analyzed.</param>
        /// <returns>Yields a tuple of asset path and progress (0..1] after each matched asset is added.</returns>
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule)
        {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = "<AssetDatabase>"
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}