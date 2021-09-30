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
using vFrame.Bundler.Modes;
using Debug = UnityEngine.Debug;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestSync : LoadRequest
    {
        internal LoadRequestSync(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader)
            : base(mode, context, path, bundleLoader) {
        }

        protected override void OnLoadProcess() {
            var stopWatch = Stopwatch.StartNew();
            LoadRecursive(_bundleLoader, 0);
            var elapse = stopWatch.Elapsed.TotalSeconds;
            //Debug.LogFormat("LoadRequestSync finished, cost: {0:0.0000}s, path: {1}", elapse, _bundleLoader.AssetBundlePath);
            IsDone = true;
        }

        private void LoadRecursive(BundleLoaderBase bundleLoader, int depth) {
            var stopWatch = Stopwatch.StartNew();

            // Load dependencies at first
            foreach (var dependency in bundleLoader.Dependencies)
                LoadRecursive(dependency, depth + 1);

            // Load target at last
            if (!bundleLoader.IsStarted) {
                bundleLoader.Load();
            }

            // Force load complete immediately if target is async loader.
            var async = bundleLoader as BundleLoaderAsync;
            if (async != null && !async.IsDone) {
                async.ForceLoadComplete();
            }

            var elapse = stopWatch.Elapsed.TotalSeconds;
            //Debug.LogFormat("Loader load finished, cost: {0:0.0000}s, path: {1}", elapse, bundleLoader.AssetBundlePath);
        }
    }
}