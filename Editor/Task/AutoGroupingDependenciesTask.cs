// ------------------------------------------------------------
//         File: AutoGroupingDependenciesTask.cs
//        Brief: AutoGroupingDependenciesTask.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:41
//    Copyright: Copyright (c) 2024, VyronLee
// ============================================================

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using vFrame.Bundler.Editor.Helper;

namespace vFrame.Bundler.Editor.Task
{
    internal class AutoGroupingDependenciesTask : BuildTaskBase
    {
        public override void Run(BuildContext context) {
            var sceneBundles = FilterMainSceneBundle(context);

            try {
                var index = 0f;
                var total = context.DependencyAssetInfos.Count;
                foreach (var kv in context.DependencyAssetInfos) {
                    var dependencyAssetInfo = kv.Value;
                    EditorUtility.DisplayProgressBar("Auto Grouping",
                        dependencyAssetInfo.AssetPath, ++index / total);

                    // Try builtin rule first.
                    if (TryBuiltinRule(context, dependencyAssetInfo.AssetPath, out var bundlePath)) {
                        dependencyAssetInfo.BundlePath = bundlePath;
                        continue;
                    }

                    // If an asset is only referenced once, it will be placed directly into
                    // the main AssetBundle without being automatically grouped.
                    if (dependencyAssetInfo.ReferenceBundles.Count == 1) {
                        var referenceBundle = dependencyAssetInfo.ReferenceBundles.First();
                        if (!IsSceneBundles(referenceBundle)) {
                            dependencyAssetInfo.BundlePath = referenceBundle;
                            continue;
                        }
                    }

                    // If an asset is referenced more than once, the automatic grouping rules
                    // will be tested to identify the names of the AssetBundle to which these dependencies belong.
                    if (!AutoGroupingFromRules(context, dependencyAssetInfo.AssetPath, out bundlePath)) {
                        AutoGroupingFromFallbackRule(dependencyAssetInfo.AssetPath, out bundlePath);
                    }
                    dependencyAssetInfo.BundlePath = context.BuildSharedBundlePath(bundlePath);
                }
            }
            finally {
                EditorUtility.ClearProgressBar();
            }
            return;

            bool IsSceneBundles(string path) {
                return sceneBundles.Contains(path);
            }
        }

        private HashSet<string> FilterMainSceneBundle(BuildContext context) {
            var ret = new HashSet<string>();
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                if (AssetHelper.IsScene(assetInfo.AssetPath)) {
                    ret.Add(assetInfo.BundlePath);
                }
            }
            return ret;
        }

        private bool TryBuiltinRule(BuildContext context, string dependencyAssetPath, out string bundlePath) {
            if (AssetHelper.IsShader(dependencyAssetPath)) {
                if (context.BuildSettings.SeparateShaderBundle) {
                    bundlePath = context.BuildSharedShaderBundlePath();
                    return true;
                }
            }

            if (AssetHelper.IsScene(dependencyAssetPath)) {
                bundlePath = context.BuildSceneBundlePath(dependencyAssetPath);
                return true;
            }

            bundlePath = "";
            return false;
        }

        private bool AutoGroupingFromRules(BuildContext context, string dependencyAssetPath, out string bundlePath) {
            bundlePath = "";

            var rules = context.BuildRules.GroupRules;
            foreach (var groupRule in rules) {
                if (!string.IsNullOrEmpty(groupRule.Exclude)) {
                    if (Regex.IsMatch(dependencyAssetPath, groupRule.Exclude)) {
                        continue;
                    }
                }
                if (!string.IsNullOrEmpty(groupRule.Include)) {
                    var regex = new Regex(groupRule.Include);
                    var match = regex.Match(dependencyAssetPath);
                    if (!match.Success) {
                        continue;
                    }
                    if (match.Groups.Count < 2) {
                        continue;
                    }
                    bundlePath = match.Groups[1].Value;
                    break;
                }
            }

            if (string.IsNullOrEmpty(bundlePath)) {
                Debug.LogWarning("None of the rules matched when testing "
                                 + $" the automatic grouping of dependencies: {dependencyAssetPath}");
                return false;
            }
            return true;
        }

        private void AutoGroupingFromFallbackRule(string dependencyAssetPath, out string bundlePath) {
            var fallbackRule = AutoGroupRule.Fallback;
            var regex = new Regex(fallbackRule.Include);
            var match = regex.Match(dependencyAssetPath);
            if (!match.Success || match.Groups.Count < 2) {
                ThrowHelper.ThrowUndesiredException("Testing fallback rule failed,"
                                                    + $" path: {dependencyAssetPath}, rule: {fallbackRule.Include}");
            }
            bundlePath = match.Groups[1].Value;
        }
    }
}