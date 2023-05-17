//------------------------------------------------------------
//        File:  LoadRequestSync.cs
//       Brief:  LoadRequestSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using System.Diagnostics;
using vFrame.Bundler.Loaders;
using vFrame.Bundler.Logs;
using vFrame.Bundler.Modes;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestSync : LoadRequest
    {
        private static readonly string[] TabOfDepth = {
            "\t",
            "\t\t",
            "\t\t\t",
            "\t\t\t\t",
            "\t\t\t\t\t",
        };

        internal LoadRequestSync(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader)
            : base(mode, context, path, bundleLoader) {
        }

        protected override void OnLoadProcess() {
            var stopWatch = Stopwatch.StartNew();
            LoadRecursive(_bundleLoader, 0);
            var elapse = stopWatch.Elapsed.TotalSeconds;
            Logger.LogVerbose("LoadRequestSync finished, cost: {0:0.0000}s, path: {1}", elapse, _bundleLoader.AssetBundlePath);
            IsDone = true;
        }

        private void LoadRecursive(BundleLoaderBase bundleLoader, int depth) {
            var stopWatch = Stopwatch.StartNew();

            // Load dependencies at first
            foreach (var dependency in bundleLoader.Dependencies) {
                LoadRecursive(dependency, depth + 1);
            }

            // Load target at last
            if (!bundleLoader.IsStarted) {
                bundleLoader.Load();
            }

            // Force load complete immediately if target is async loader.
            var async = bundleLoader as BundleLoaderAsync;
            if (async != null && !async.IsDone) {
                async.ForceLoadComplete();
            }

            var tabStr = TabOfDepth.Length > depth ? TabOfDepth[depth] : new string('\t', depth);
            var elapse = stopWatch.Elapsed.TotalSeconds;
            Logger.LogVerbose("{0}Loader load finished, cost: {1:0.0000}s, path: {2}", tabStr, elapse, bundleLoader.AssetBundlePath);
        }
    }
}