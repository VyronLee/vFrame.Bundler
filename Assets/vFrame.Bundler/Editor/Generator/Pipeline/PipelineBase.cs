// ------------------------------------------------------------
//         File: PipelineBase.cs
//        Brief: Base bundle build pipeline: validates rules and settings, then runs each build task in
//               sequence with elapsed-time logging.
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//     Modified: 2026-09-22 06:17:26
//    Copyright: Copyright (c) 2026, VyronLee
// ============================================================


using System;
using System.Diagnostics;

namespace vFrame.Bundler.Editor
{
    /// <summary>
    ///     Base implementation of an asset bundle build pipeline. Validates the supplied build rules and
    ///     settings, then executes the pipeline's tasks in order.
    /// </summary>
    internal abstract class PipelineBase : IPipeline
    {
        /// <summary>
        ///     Runs the pipeline: validates the build rules and settings, executes each task returned by
        ///     <see cref="GetTasks" /> in order, and logs the total elapsed time.
        /// </summary>
        /// <param name="buildRules">Rules describing main assets and dependency grouping.</param>
        /// <param name="buildSettings">Settings describing output paths and bundle name formatters.</param>
        /// <exception cref="BundleArgumentException">Thrown when rules or settings are null or contain empty values.</exception>
        public void Build(BundleBuildRules buildRules, BundleBuildSettings buildSettings)
        {
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

        /// <summary>
        ///     Ensures that the build rules and all their required fields (main rules and group rules) are
        ///     present and non-empty.
        /// </summary>
        /// <param name="buildRules">Rules to validate.</param>
        /// <exception cref="BundleArgumentException">Thrown when a rule or one of its required fields is null or empty.</exception>
        private static void ValidateBuildRules(BundleBuildRules buildRules)
        {
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

        /// <summary>
        ///     Ensures that the build settings and all their required fields (output paths and bundle name
        ///     formatters) are present and non-empty.
        /// </summary>
        /// <param name="buildSettings">Settings to validate.</param>
        /// <exception cref="BundleArgumentException">Thrown when the settings or one of their required fields is null or empty.</exception>
        private static void ValidateBuildSettings(BundleBuildSettings buildSettings)
        {
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

        /// <summary>
        ///     Creates the ordered list of build tasks this pipeline executes.
        /// </summary>
        /// <returns>The tasks to run in order, or null to run no tasks.</returns>
        protected abstract BuildTaskBase[] GetTasks();
    }
}