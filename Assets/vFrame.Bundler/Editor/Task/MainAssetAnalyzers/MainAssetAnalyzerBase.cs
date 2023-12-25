// ------------------------------------------------------------
//         File: MainAssetAnalyzerBase.cs
//        Brief: MainAssetAnalyzerBase.cs
//
//       Author: VyronLee, lwz_jz@hotmail.com
//
//      Created: 2023-12-25 22:51
//    Copyright: Copyright (c) 2023, VyronLee
// ============================================================

using System.Collections.Generic;
using UnityEditor;

namespace vFrame.Bundler.Editor.Task.MainAssetAnalyzers
{
    internal abstract class MainAssetAnalyzerBase
    {
        public void Run(BuildContext context, MainBundleRule rule) {
            ThrowHelper.ThrowIfNullOrEmpty(rule.SearchPath,
                ThrowHelper.Variables(nameof(rule), nameof(rule.SearchPath)));
            ThrowHelper.ThrowIfNullOrEmpty(rule.Include,
                ThrowHelper.Variables(nameof(rule), nameof(rule.Include)));
            try {
                var title = $"Analyzing: ${rule.SearchPath}";
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
    }
}