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
        public LoadRequestSync(ModeBase mode, BundlerOptions options, string path, BundleLoaderBase bundleLoader)
            : base(mode, options, path, bundleLoader) {
        }

        public override bool IsDone {
            get { return _bundleLoader == null || _bundleLoader.IsDone; }
        }

        protected override void LoadInternal() {
            LoadRecursive(_bundleLoader);
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