// ------------------------------------------------------------
//         File: MainAssetAnalyzerBase.cs
//        Brief: MainAssetAnalyzerBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:51
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using vFrame.Bundler.Helper;

namespace vFrame.Bundler.Task
{
    internal abstract class MainAssetAnalyzerBase
    {
        public void Run(BuildContext context, MainBundleRule rule) {
            ThrowHelper.ThrowIfNullOrEmpty(rule.SearchPath,
                ThrowHelper.Variables(nameof(rule), nameof(rule.SearchPath)));
            ThrowHelper.ThrowIfNullOrEmpty(rule.Include,
                ThrowHelper.Variables(nameof(rule), nameof(rule.Include)));
            try {
                var title = $"Analyzing: {rule.SearchPath}";
                var iter = OnRun(context, rule);
                while (iter.MoveNext()) {
                    var (info, percent) = iter.Current;
                    EditorUtility.DisplayProgressBar(title, info, percent);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
        }

        protected abstract IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule);

        protected List<string> FindAssets(MainBundleRule rule) {
            try {
                EditorUtility.DisplayProgressBar("Finding Assets", rule.SearchPath, 0.2f);
                var assets = AssetHelper.FindAllAssets(rule.SearchPath).Where(FilterTest).ToList();
                assets.Sort();
                return assets;
            }
            finally {
                EditorUtility.ClearProgressBar();
            }

            bool FilterTest(string path) {
                return IsFilteringTestPassed(rule, path);
            }
        }

        protected void SafeAddMainAssetInfo(BuildContext context, MainAssetInfo assetInfo) {
            if (context.MainAssetInfos.TryGetValue(assetInfo.AssetPath, out var info)) {
                Debug.LogWarning($"Skip because asset already contains in bundle: {info.BundlePath}");
                return;
            }
            context.MainAssetInfos.Add(assetInfo.AssetPath, assetInfo);
        }

        private bool IsFilteringTestPassed(MainBundleRule rule, string assetPath) {
            // Include
            if (string.IsNullOrEmpty(rule.Include)) {
                return false;
            }
            var include = Regex.IsMatch(assetPath, rule.Include);
            if (!include) {
                return false;
            }

            // Exclude
            if (string.IsNullOrEmpty(rule.Exclude)) {
                return true;
            }
            var exclude = Regex.IsMatch(assetPath, rule.Exclude);
            return !exclude;
        }
    }
}