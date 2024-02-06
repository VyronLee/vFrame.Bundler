// ------------------------------------------------------------
//         File: AnalyzePackByAllFilesRule.cs
//        Brief: AnalyzePackByAllFilesRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-28 16:49
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Task.Simulation.MainAssetAnalyzers
{
    internal class AnalyzePackByAllFilesRule : SimulationMainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
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