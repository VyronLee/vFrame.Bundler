// ------------------------------------------------------------
//         File: MainAssetAnalyzerBase.cs
//        Brief: Shared base for main-asset analyzers; drives analyzer iterators behind a progress bar,
//               filters assets via Include/Exclude regex, and guards against duplicate registration.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:22:03
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Shared base for main-asset analyzers. Drives the analyzer's iterator with an editor
    ///     progress bar and provides rule-based asset searching and duplicate-guarded registration.
    /// </summary>
    internal abstract class MainAssetAnalyzerBase
    {
        /// <summary>
        ///     Runs the analyzer, forwarding each progress tuple yielded by <see cref="OnRun"/> to an
        ///     editor progress bar that is always cleared afterwards.
        /// </summary>
        /// <param name="context">Build context receiving the analyzed main-asset infos.</param>
        /// <param name="rule">Main bundle rule providing the search path and Include/Exclude patterns.</param>
        /// <exception cref="BundleArgumentException">
        ///     Thrown when the rule's search path or include pattern is null or empty.
        /// </exception>
        public void Run(BuildContext context, MainBundleRule rule)
        {
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

        /// <summary>Implements the analysis, yielding a status message and completion fraction per step.</summary>
        /// <param name="context">Build context receiving the analyzed main-asset infos.</param>
        /// <param name="rule">Main bundle rule providing the search path and Include/Exclude patterns.</param>
        /// <returns>Iterator yielding (progress-bar info, completion fraction) tuples.</returns>
        protected abstract IEnumerator<(string, float)> OnRun(BuildContext context, MainBundleRule rule);

        /// <summary>Finds all assets under the rule's search path passing the Include/Exclude filter, sorted by path.</summary>
        /// <param name="rule">Main bundle rule providing the search path and Include/Exclude patterns.</param>
        /// <returns>Sorted asset paths that pass the filter.</returns>
        protected List<string> FindAssets(MainBundleRule rule)
        {
            try {
                EditorUtility.DisplayProgressBar("Finding Assets", rule.SearchPath, 0.2f);
                var assets = AssetHelper.FindAllAssets(rule.SearchPath).Where(FilterTest).ToList();
                assets.Sort();
                return assets;
            }
            finally {
                EditorUtility.ClearProgressBar();
            }

            bool FilterTest(string path)
            {
                return IsFilteringTestPassed(rule, path);
            }
        }

        /// <summary>Adds the asset info to the context unless an asset with the same path is already registered.</summary>
        /// <param name="context">Build context receiving the main-asset info.</param>
        /// <param name="assetInfo">Main-asset info to register, keyed by its asset path.</param>
        protected void SafeAddMainAssetInfo(BuildContext context, MainAssetInfo assetInfo)
        {
            if (context.MainAssetInfos.TryGetValue(assetInfo.AssetPath, out var info)) {
                Debug.LogWarning($"Skip because asset already contains in bundle: {info.BundlePath}");
                return;
            }
            context.MainAssetInfos.Add(assetInfo.AssetPath, assetInfo);
        }

        /// <summary>Returns whether the asset path matches the rule's Include regex and not its Exclude regex.</summary>
        /// <param name="rule">Main bundle rule providing the Include/Exclude regex patterns.</param>
        /// <param name="assetPath">Asset path to test.</param>
        /// <returns>True if the path passes the filter; otherwise false.</returns>
        private bool IsFilteringTestPassed(MainBundleRule rule, string assetPath)
        {
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