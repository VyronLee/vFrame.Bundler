// ------------------------------------------------------------
//         File: AnalyzePackBySingleFileRule.cs
//        Brief: AnalyzePackBySingleFileRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:53
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Task.Simulation.MainAssetAnalyzers
{
    internal class AnalyzePackBySingleFileRule : SimulationMainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
        }
    }
}