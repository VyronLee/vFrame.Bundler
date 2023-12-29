// ------------------------------------------------------------
//         File: AnalyzePackByAllFilesRule.cs
//        Brief: AnalyzePackByAllFilesRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-28 16:49
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;

namespace vFrame.Bundler.Editor.Task.MainAssetAnalyzers
{
    internal class AnalyzePackByAllFilesRule : MainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
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