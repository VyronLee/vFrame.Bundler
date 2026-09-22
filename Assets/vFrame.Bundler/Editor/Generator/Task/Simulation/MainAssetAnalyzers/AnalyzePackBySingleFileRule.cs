// ------------------------------------------------------------
//         File: AnalyzePackBySingleFileRule.cs
//        Brief: Simulation analyzer: maps each PackBySingleFile rule match to the <AssetDatabase> pseudo bundle.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:22:10
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Simulation analyzer that registers every asset matched by a <see cref="MainBundleRule"/> of pack type
    ///     <c>PackBySingleFile</c> as a main asset of the "&lt;AssetDatabase&gt;" pseudo bundle.
    /// </summary>
    internal class SimulationAnalyzePackBySingleFileRule : SimulationMainAssetAnalyzerBase
    {
        /// <summary>
        ///     Enumerates the assets matched by <paramref name="rule"/>, yielding per-asset progress and registering
        ///     each one as a main asset of the "&lt;AssetDatabase&gt;" pseudo bundle.
        /// </summary>
        /// <param name="context">Build context collecting the registered main asset infos.</param>
        /// <param name="rule">Main bundle rule whose matched assets are mapped to the pseudo bundle.</param>
        /// <returns>
        ///     A sequence of (asset path, progress) pairs whose progress rises from <c>1/N</c> to <c>1</c> as the
        ///     assets are registered; empty when no asset matches the rule.
        /// </returns>
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