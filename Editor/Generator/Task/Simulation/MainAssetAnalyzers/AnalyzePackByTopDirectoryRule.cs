// ------------------------------------------------------------
//         File: AnalyzePackByTopDirectoryRule.cs
//        Brief: AnalyzePackByTopDirectoryRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:54
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.IO;

namespace vFrame.Bundler.Task.Simulation.MainAssetAnalyzers
{
    internal class AnalyzePackByTopDirectoryRule : SimulationMainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
            var assets = FindAssets(rule);
            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                if (!IsTopDirectoryAsset(asset)) {
                    continue;
                }
                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = "<AssetDatabase>"
                };
                SafeAddMainAssetInfo(context, assetInfo);
            }
            yield break;

            bool IsTopDirectoryAsset(string path) {
                var dirName = Path.GetDirectoryName(path);
                if (string.IsNullOrEmpty(dirName)) {
                    return false;
                }
                return dirName == rule.SearchPath;
            }
        }
    }
}