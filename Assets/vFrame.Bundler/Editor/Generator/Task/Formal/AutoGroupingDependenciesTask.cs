// ------------------------------------------------------------
//         File: AutoGroupingDependenciesTask.cs
//        Brief: Build step 3: assigns every shared dependency to a bundle via builtin shader/scene rules,
//               single-reference inlining, GroupRules regex matching, or the fallback rule.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:19:33
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
    /// Build pipeline task that assigns each shared dependency asset to a target bundle:
    /// builtin shader/scene rules first, then single-reference inlining into the referencing
    /// bundle, then <see cref="BundleBuildRules.GroupRules"/> regex matching, and finally the
    /// fallback rule when nothing else matched.
    /// </summary>
    internal class AutoGroupingDependenciesTask : BuildTaskBase
    {
        /// <summary>
        /// Assigns a bundle path to every shared dependency asset info in the context.
        /// </summary>
        /// <param name="context">Build context carrying dependency asset infos and grouping rules.</param>
        public override void Run(BuildContext context)
        {
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

            bool IsSceneBundles(string path)
            {
                return sceneBundles.Contains(path);
            }
        }

        /// <summary>
        /// Collects the bundle paths of all main assets that are scenes, so dependency
        /// scenes can be told apart from regular referencing bundles.
        /// </summary>
        /// <param name="context">Build context carrying main asset infos.</param>
        /// <returns>Set of bundle paths that contain main scene assets.</returns>
        private HashSet<string> FilterMainSceneBundle(BuildContext context)
        {
            var ret = new HashSet<string>();
            foreach (var kv in context.MainAssetInfos) {
                var assetInfo = kv.Value;
                if (AssetHelper.IsScene(assetInfo.AssetPath)) {
                    ret.Add(assetInfo.BundlePath);
                }
            }
            return ret;
        }

        /// <summary>
        /// Applies the builtin grouping rules: shaders go into the shared shader bundle when
        /// <see cref="BundleBuildSettings.SeparateShaderBundle"/> is enabled, and scenes go
        /// into their own per-scene bundle.
        /// </summary>
        /// <param name="context">Build context carrying build settings and shared bundle paths.</param>
        /// <param name="dependencyAssetPath">Asset path of the shared dependency to group.</param>
        /// <param name="bundlePath">Target bundle path when the method returns true; empty otherwise.</param>
        /// <returns>True if a builtin rule matched the dependency; otherwise false.</returns>
        private bool TryBuiltinRule(BuildContext context, string dependencyAssetPath, out string bundlePath)
        {
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

        /// <summary>
        /// Groups the dependency by matching it against the configured <see cref="BundleBuildRules.GroupRules"/>;
        /// the first include regex with a capture group provides the bundle name.
        /// </summary>
        /// <param name="context">Build context carrying the group rules.</param>
        /// <param name="dependencyAssetPath">Asset path of the shared dependency to group.</param>
        /// <param name="bundlePath">Matched bundle name when the method returns true; empty otherwise.</param>
        /// <returns>True if a group rule matched the dependency; otherwise false.</returns>
        private bool AutoGroupingFromRules(BuildContext context, string dependencyAssetPath, out string bundlePath)
        {
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

        /// <summary>
        /// Groups the dependency using the <see cref="AutoGroupRule.Fallback"/> rule; its include
        /// regex must match with a capture group that yields the bundle name.
        /// </summary>
        /// <param name="dependencyAssetPath">Asset path of the shared dependency to group.</param>
        /// <param name="bundlePath">Bundle name extracted from the fallback rule match.</param>
        /// <exception cref="BundleException">Thrown when the fallback rule regex fails to match.</exception>
        private void AutoGroupingFromFallbackRule(string dependencyAssetPath, out string bundlePath)
        {
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