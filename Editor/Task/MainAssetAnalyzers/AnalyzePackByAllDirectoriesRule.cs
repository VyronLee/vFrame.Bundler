// ------------------------------------------------------------
//         File: AnalyzePackByAllDirectoriesRule.cs
//        Brief: AnalyzePackByAllDirectoriesRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:54
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.IO;

namespace vFrame.Bundler.Editor.Task.MainAssetAnalyzers
{
    internal class AnalyzePackByAllDirectoriesRule : MainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
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