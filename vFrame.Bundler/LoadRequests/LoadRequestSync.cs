//------------------------------------------------------------
//        File:  LoadRequestSync.cs
//       Brief:  LoadRequestSync
//
//      Author:  VyronLee, lwz_jz@hotmail.com
//
//    Modified:  2019-02-15 20:50
//   Copyright:  Copyright (c) 2019, VyronLee
//============================================================

using vFrame.Bundler.Loaders;
using vFrame.Bundler.Modes;

namespace vFrame.Bundler.LoadRequests
{
    public sealed class LoadRequestSync : LoadRequest
    {
        internal LoadRequestSync(ModeBase mode, BundlerContext context, string path, BundleLoaderBase bundleLoader)
            : base(mode, context, path, bundleLoader) {
        }

        protected override void OnLoadProcess() {
            LoadRecursive(_bundleLoader);
            IsDone = true;
        }

        private void LoadRecursive(BundleLoaderBase bundleLoader) {
            // Load dependencies at first
            foreach (var dependency in bundleLoader.Dependencies)
                LoadRecursive(dependency);

            // Load target at last
            if (!bundleLoader.IsStarted) {
                bundleLoader.Load();
            }

            // Force load complete immediately if target is async loader.
            var async = bundleLoader as BundleLoaderAsync;
            if (async != null && !async.IsDone) {
                async.ForceLoadComplete();
            }
        }
    }
}