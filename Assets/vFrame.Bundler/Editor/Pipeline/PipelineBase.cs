// ------------------------------------------------------------
//         File: PipelineBase.cs
//        Brief: PipelineBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-26 22:12
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System;
using System.Diagnostics;
using vFrame.Bundler.Editor.Helper;
using vFrame.Bundler.Editor.Task;

namespace vFrame.Bundler.Editor.Pipeline
{
    internal abstract class PipelineBase : IPipeline
    {
        public void Build(BundleBuildRules buildRules, BundleBuildSettings buildSettings) {
            ValidateBuildRules(buildRules);
            ValidateBuildSettings(buildSettings);

            var stopWatch = Stopwatch.StartNew();
            var buildContext = new BuildContext {
                BuildRules = buildRules,
                BuildSettings = buildSettings
            };
            var tasks = GetTasks() ?? Array.Empty<BuildTaskBase>();
            foreach (var task in tasks) {
                task?.Run(buildContext);
            }

            UnityEngine.Debug.Log($"Bundle build finished, cost: {stopWatch.Elapsed.TotalSeconds:F1}s.");
        }

        private static void ValidateBuildRules(BundleBuildRules buildRules) {
            ThrowHelper.ThrowIfNull(buildRules, nameof(buildRules));

            ThrowHelper.ThrowIfNull(buildRules.MainRules,
                ThrowHelper.Variables(nameof(buildRules), nameof(buildRules.MainRules)));
            foreach (var mainRule in buildRules.MainRules) {
                ThrowHelper.ThrowIfNullOrEmpty(mainRule.PackType, nameof(mainRule.PackType));
                ThrowHelper.ThrowIfNullOrEmpty(mainRule.SearchPath, nameof(mainRule.SearchPath));
                ThrowHelper.ThrowIfNullOrEmpty(mainRule.Include, nameof(mainRule.Include));
            }

            ThrowHelper.ThrowIfNull(buildRules.GroupRules,
                ThrowHelper.Variables(nameof(buildRules), nameof(buildRules.GroupRules)));
            foreach (var groupRule in buildRules.GroupRules) {
                ThrowHelper.ThrowIfNullOrEmpty(groupRule.Include, nameof(groupRule.Include));
            }
        }

        private static void ValidateBuildSettings(BundleBuildSettings buildSettings) {
            ThrowHelper.ThrowIfNull(buildSettings, nameof(buildSettings));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.BundlePath,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.BundlePath)));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.ManifestFileName,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.ManifestFileName)));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.BundleFormatter,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.BundleFormatter)));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.SharedBundleFormatter,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.SharedBundleFormatter)));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.SceneBundleFormatter,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.SceneBundleFormatter)));
            ThrowHelper.ThrowIfNullOrEmpty(buildSettings.SeparatedShaderBundlePath,
                ThrowHelper.Variables(nameof(buildSettings), nameof(buildSettings.SeparatedShaderBundlePath)));
        }

        protected abstract BuildTaskBase[] GetTasks();
    }
}