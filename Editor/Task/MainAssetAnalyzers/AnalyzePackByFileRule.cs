// ------------------------------------------------------------
//         File: AnalyzePackByFileRule.cs
//        Brief: AnalyzePackByFileRule.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:53
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using vFrame.Bundler.Utils;

namespace vFrame.Bundler.Editor.Task.MainAssetAnalyzers
{
    internal class AnalyzePackByFileRule : MainAssetAnalyzerBase
    {
        protected override IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule) {
            var assets = AssetDatabase.FindAssets("*.*", new[] { rule.SearchPath })
                .Select(AssetHelper.GuidToPath)
                .ToList();

            var index = 0f;
            var total = assets.Count;
            foreach (var asset in assets) {
                yield return (asset, ++index / total);

                var bundleName = PathUtility.NormalizeAssetBundlePath(asset);
                bundleName = string.Format(context.BuildSettings.BundleFormatter, bundleName);

                var assetInfo = new MainAssetInfo {
                    AssetPath = asset,
                    BundlePath = bundleName
                };
                context.MainAssetInfos.Add(assetInfo);
            }
        }
    }
}